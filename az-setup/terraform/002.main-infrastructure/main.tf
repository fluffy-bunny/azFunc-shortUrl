terraform {
  backend "azurerm" {
    # Due to a limitation in backend objects, variables cannot be passed in.
    # Do not declare an access_key here. Instead, export the
    # ARM_ACCESS_KEY environment variable.

    storage_account_name  = "stterraformshorturl"
    container_name        = "tstate"
    key                   = "terraform.tfstate"
  }
}
# Configure the Azure provider
provider "azurerm" {
  version = "=1.44.0"
}
resource "random_string" "suffix" {
  length  = 10
  upper   = false
  special = false
}
locals {
  resource_suffix              = random_string.suffix.result
}
data "azurerm_subscription" "primary" {}
data "azurerm_role_definition" "contributor" {
  name = "Contributor"
}
data "azurerm_role_definition" "Azure_Service_Bus_Data_Owner" {
  name = "Azure Service Bus Data Owner"
}
data "azurerm_role_definition" "Azure_Event_Hubs_Data_Owner" {
  name = "Azure Event Hubs Data Owner"
}
data "azurerm_role_definition" "Azure_Storage_Queue_Data_Contributor" {
  name = "Storage Queue Data Contributor"
}

resource "azurerm_resource_group" "rg" {
  name     = var.az_resource_group_name
  location = var.az_resource_group_location
}

resource "azurerm_application_insights" "main" {
  name                = "appis-shorturl"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_cosmosdb_account" "main" {
  name                = "cosmos-shorturl"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  enable_automatic_failover = false

  consistency_policy {
    consistency_level       = "Eventual"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100
  }

  geo_location {
    prefix            = "cosmos-shorturl-customid"
    location          = azurerm_resource_group.rg.location
    failover_priority = 0
  }
}
resource "azurerm_cosmosdb_sql_database" "shorturl_db" {
  name                = "shorturl"
  resource_group_name = azurerm_cosmosdb_account.main.resource_group_name
  account_name        = azurerm_cosmosdb_account.main.name
  throughput          = 400
}
resource "azurerm_cosmosdb_sql_container" "shorturl_container" {
  name                = "shorturl"
  resource_group_name = azurerm_cosmosdb_account.main.resource_group_name
  account_name        = azurerm_cosmosdb_account.main.name
  database_name       = azurerm_cosmosdb_sql_database.shorturl_db.name
  partition_key_path  = "/id"
  throughput          = 400
  default_ttl         = -1 
}
resource "azurerm_cosmosdb_sql_container" "expired_shorturl_container" {
  name                = "expired-shorturl"
  resource_group_name = azurerm_cosmosdb_account.main.resource_group_name
  account_name        = azurerm_cosmosdb_account.main.name
  database_name       = azurerm_cosmosdb_sql_database.shorturl_db.name
  partition_key_path  = "/id"
  throughput          = 400
  default_ttl         = -1 
}
 
resource "azurerm_storage_account" "azfunc_shorturl" {
  name                     = "stazfuncshorturl"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "evh_blob" {
  name                      = "evh-blob"
  resource_group_name       = azurerm_resource_group.rg.name
  storage_account_name      = azurerm_storage_account.azfunc_shorturl.name
  container_access_type     = "private"
}

resource "azurerm_storage_queue" "shorturl_backend_processing" {
  name                 = "backend-processing"
  storage_account_name = azurerm_storage_account.azfunc_shorturl.name
}

resource "azurerm_app_service_plan" "azfunc_consumption" {
  name                = "plan-shorturl"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}
resource "azurerm_function_app" "azfunc_shorturl" {
  name                      = "azfunc-shorturl"
  location                  = azurerm_resource_group.rg.location
  resource_group_name       = azurerm_resource_group.rg.name
  app_service_plan_id       = azurerm_app_service_plan.azfunc_consumption.id
  storage_connection_string = azurerm_storage_account.azfunc_shorturl.primary_connection_string
  identity { type = "SystemAssigned" }
  app_settings = {
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                   = "true",
    "WEBSITE_RUN_FROM_PACKAGE"                          = "1",
    "APPINSIGHTS_INSTRUMENTATIONKEY"                    = azurerm_application_insights.main.instrumentation_key,
    "APPLICATIONINSIGHTS_CONNECTION_STRING"             = format("InstrumentationKey=%s", azurerm_application_insights.main.instrumentation_key),
    "azFunc-shorturl-cosmos-primary-connection-string"  = var.azFunc_shorturl_cosmos_primary_connection_string,
    "azFunc-shorturl-cosmos-primarykey"                 = var.azFunc_shorturl_cosmos_primarykey,
    "azFunc-shorturl-cosmos-uri"                        = var.azFunc_shorturl_cosmos_uri,
    "jwt-validate-settings"                             = var.jwt_validate_settings,
    "keyvault-config"                                   = var.keyvault_config,
    "FUNCTIONS_WORKER_RUNTIME"                          = "dotnet"
  }
  version="~3"

}

resource "azurerm_servicebus_namespace" "shorturl" {
  name                = "sbns-shorturl"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"

  tags = {
    source = "terraform"
  }
}

resource "azurerm_servicebus_topic" "shorturl" {
  name                = "sbtpc-shorturl"
  resource_group_name = azurerm_resource_group.rg.name
  namespace_name      = azurerm_servicebus_namespace.shorturl.name

  enable_partitioning = true
}

resource "azurerm_servicebus_queue" "shorturl" {
  name                = "sbque-shorturl"
  resource_group_name = azurerm_resource_group.rg.name
  namespace_name      = azurerm_servicebus_namespace.shorturl.name

  enable_partitioning = true
}


resource "azurerm_eventhub_namespace" "evt_namespace_shorturl" {
  name                = "evhns-shorturl-001"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"
  capacity            = 1

  tags = {
    environment = "Production"
  }
}
resource "azurerm_eventhub" "evh_shorturl" {
  name                = "evh-shorturl"
  namespace_name      = azurerm_eventhub_namespace.evt_namespace_shorturl.name
  resource_group_name = azurerm_resource_group.rg.name
  partition_count     = 2
  message_retention   = 1
}


resource "azurerm_eventgrid_topic" "tpc_shorturl_tenant_usage" {
  name                = "tpc-shorturl-tenant-usage"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  tags = {
    environment = "Production"
  }
}

resource "azurerm_role_assignment" "evh_data_owner_azfun" {
  name               = var.role_assignment_evh_data_owner_azfun_name
  scope              = azurerm_eventhub_namespace.evt_namespace_shorturl.id
  role_definition_id = data.azurerm_role_definition.Azure_Event_Hubs_Data_Owner.id
  principal_id       = azurerm_function_app.azfunc_shorturl.identity.0.principal_id
}

resource "azurerm_role_assignment" "sb_data_owner_azfun" {
  name               = var.role_assignment_sb_data_owner_azfun_name
  scope              = azurerm_servicebus_namespace.shorturl.id
  role_definition_id = data.azurerm_role_definition.Azure_Service_Bus_Data_Owner.id
  principal_id       = azurerm_function_app.azfunc_shorturl.identity.0.principal_id
}

resource "azurerm_role_assignment" "storage_queue_data_owner_azfun" {
  name               = var.role_assignment_storage_queue_data_owner_azfun_name
  scope              = azurerm_storage_account.azfunc_shorturl.id
  role_definition_id = data.azurerm_role_definition.Azure_Storage_Queue_Data_Contributor.id
  principal_id       = azurerm_function_app.azfunc_shorturl.identity.0.principal_id
}



module "key_vault" {
  source = "innovationnorway/key-vault/azurerm"

  name = var.az_keyvault_name

  resource_group_name = azurerm_resource_group.rg.name

  access_policies = [
      {
        user_principal_names = ["hstahl@symantec.com"]
        secret_permissions   = ["backup","delete","get", "list","purge","recover","restore","set"]
      },
      {
        object_ids        = ["${azurerm_function_app.azfunc_shorturl.identity.0.principal_id}"]
        secret_permissions   = ["get"]
      },
      
  ]


  secrets = {
    "message" = "Hello, world!"
  }
}

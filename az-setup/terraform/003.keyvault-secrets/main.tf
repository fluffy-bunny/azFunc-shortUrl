terraform {
  backend "azurerm" {
    # Due to a limitation in backend objects, variables cannot be passed in.
    # Do not declare an access_key here. Instead, export the
    # ARM_ACCESS_KEY environment variable.

    storage_account_name  = "stterraformshorturl"
    container_name        = "tstate"
    key                   = "terraform.tfstate.keyvault.secrets"
  }
}
# Configure the Azure provider
provider "azurerm" {
  version = "=1.44.0"
}

data "azurerm_resource_group" "rg" {
  name     = var.az_resource_group_name
}
data "azurerm_key_vault" "main" {
  name = var.az_keyvault_name
  resource_group_name         = data.azurerm_resource_group.rg.name
}
data "azurerm_cosmosdb_account" "main" {
  name = "cosmos-shorturl"
  resource_group_name         = data.azurerm_resource_group.rg.name
}

resource "azurerm_key_vault_secret" "main" {
  for_each     = {
    "message" = "Hello, world!",
    "azFunc-shorturl-client-credentials" = var.azFunc_shorturl_client_credentials,
    "azFunc-shorturl-cosmos-primary-connection-string" = format("AccountEndpoint=%s;AccountKey=%s;",data.azurerm_cosmosdb_account.main.endpoint,data.azurerm_cosmosdb_account.main.primary_master_key),
    "azFunc-shorturl-cosmos-primarykey" = data.azurerm_cosmosdb_account.main.primary_master_key,
    "azFunc-shorturl-cosmos-uri"        = data.azurerm_cosmosdb_account.main.endpoint,
    "jwt-validate-settings"             = var.jwt_validate_settings,
  }
  name         = each.key
  value        = each.value
  key_vault_id = data.azurerm_key_vault.main.id
}

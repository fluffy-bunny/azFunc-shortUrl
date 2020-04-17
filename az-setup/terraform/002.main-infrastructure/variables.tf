variable "az_resource_group_name" {
  description = "(Required) The name of the resource group where resources will be created."
  type        = string
}

variable "az_resource_group_location" {
  description = "(Required) The location where the resource group will reside."
  type        = string
}
variable "az_keyvault_name" {
  description = "(Required) The main keyvault."
  type        = string
}
variable "azFunc_shorturl_name" {
  description = "(Required) The az function shorturl name."
  type        = string
}
variable "azFunc_shorturl_cosmos_primary_connection_string" {
  description = "(Required) azFunc_shorturl_cosmos_primary_connection_string."
  type        = string
}
variable "keyvault_config" {
  description = "(Required) The base64 keyvault_config."
  type        = string
}
variable "azFunc_shorturl_cosmos_primarykey" {
  description = "(Required) azFunc_shorturl_cosmos_primarykey."
  type        = string
}
variable "azFunc_shorturl_cosmos_uri" {
  description = "(Required) azFunc_shorturl_cosmos_uri."
  type        = string
}

variable "jwt_validate_settings" {
  description = "(Required) The base64 jwt_validate_settings."
  type        = string
}

variable "role_assignment_evh_data_owner_azfun_name" {
  description = "(Required) Unique GUID name of a role assignment."
  type        = string
}
variable "role_assignment_sb_data_owner_azfun_name" {
  description = "(Required) Unique GUID name of a role assignment."
  type        = string
}
variable "role_assignment_storage_queue_data_owner_azfun_name" {
  description = "(Required) Unique GUID name of a role assignment."
  type        = string
}




variable "tags" {
  description = "Tags to help identify various services."
  type        = map
  default = {
    DeployedBy     = "terraform"
    Environment    = "prod"
    OwnerEmail     = "DL-P7-OPS@p7.com"
    Platform       = "na" # does not apply to us.
  }
}
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
variable "azFunc_shorturl_client_credentials" {
  description = "(Required) The azFunc_shorturl_client_credentials name."
  type        = string
}
variable "jwt_validate_settings" {
  description = "(Required) The base64 jwt_validate_settings."
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

variable "secrets" {
  type        = map(string)
  description = "A map of secrets for the Key Vault."
  default     = {}
}
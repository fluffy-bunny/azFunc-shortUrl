variable "az_resource_group_name" {
  description = "(Required) The name of the resource group where resources will be created."
  type        = string
}

variable "az_resource_group_location" {
  description = "(Required) The location where the resource group will reside."
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
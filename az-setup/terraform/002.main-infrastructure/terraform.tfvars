az_resource_group_location = "eastus2"
az_resource_group_name = "rg-shorturl"
az_keyvault_name = "kv-shorturl"
azFunc_shorturl_name = "azFunc-shorturl"
role_assignment_evh_data_owner_azfun_name = "0db158bd-d0b4-42d3-b97a-20bbf6e47591"
role_assignment_sb_data_owner_azfun_name  = "4b3cf9b8-f184-442a-93cc-e13a2ba994e4"
role_assignment_storage_queue_data_owner_azfun_name  = "03c33aac-ac4a-4d63-b0ac-c7a7d1a56640"
 



azFunc_shorturl_cosmos_primary_connection_string    = "@Microsoft.KeyVault(SecretUri=https://kv-shorturl.vault.azure.net/secrets/azFunc-shorturl-cosmos-primary-connection-string/90bdf88a107a43548d9d7c3c3ba30dd9)"
azFunc_shorturl_cosmos_primarykey                   = "@Microsoft.KeyVault(SecretUri=https://kv-shorturl.vault.azure.net/secrets/azFunc-shorturl-cosmos-primarykey/35c430646a274e9fb9d78c086ceae15c)"
azFunc_shorturl_cosmos_uri                          = "@Microsoft.KeyVault(SecretUri=https://kv-shorturl.vault.azure.net/secrets/azFunc-shorturl-cosmos-uri/c47df5ab96284527947e4b9292e3259b)"
jwt_validate_settings                               = "@Microsoft.KeyVault(SecretUri=https://kv-shorturl.vault.azure.net/secrets/jwt-validate-settings/621ecb8ae02c4a98912f5b7413798137)"
keyvault_config                                     = "eyJleHBpcmF0aW9uU2Vjb25kcyI6MzAwLCJrZXlWYXVsdE5hbWUiOiJrdi1zaG9ydHVybCIsInNlY3JldE5hbWUiOiJhekZ1bmMtc2hvcnR1cmwtY2xpZW50LWNyZWRlbnRpYWxzIn0="


# export ARM_ACCESS_KEY=$(az keyvault secret show --name terraform-backend-key --vault-name kv-terraform-shorturl --query value -o tsv)  
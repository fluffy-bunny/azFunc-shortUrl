az_resource_group_location = "eastus2"
az_resource_group_name = "rg-shorturl"
az_keyvault_name = "kv-shorturl"
azFunc_shorturl_name = "azFunc-shorturl"



# export ARM_ACCESS_KEY=$(az keyvault secret show --name terraform-backend-key --vault-name kv-terraform-shorturl --query value -o tsv)  
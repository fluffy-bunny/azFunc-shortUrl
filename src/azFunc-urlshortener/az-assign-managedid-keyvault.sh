
AZ_FUNC_NAME="azFunc-urlshortener"
APP_FRIENDLY_NAME="organics"
LOCATION="eastus2"
RESOURCE_GROUP_NAME="rg-$APP_FRIENDLY_NAME-openhack"
KEYVAULT_NAME="kv-$APP_FRIENDLY_NAME"


az webapp identity assign \
    --name $AZ_FUNC_NAME  \
    --resource-group $RESOURCE_GROUP_NAME
PRINCIPAL_ID=$(az webapp show -n $AZ_FUNC_NAME -g $RESOURCE_GROUP_NAME --query 'identity.principalId' -o json)
echo "KEYVAULT_NAME:$KEYVAULT_NAME"
echo "principalId:$PRINCIPAL_ID"



az keyvault set-policy \
    --name $KEYVAULT_NAME \
    --object-id "9a72878c-b0b8-461d-9259-1034beb9254a" \
    --secret-permissions get list 
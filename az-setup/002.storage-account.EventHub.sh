# https://docs.microsoft.com/en-us/azure/event-grid/custom-event-to-queue-storage

RG_FRIENDLY_NAME="shorturl"
APP_SHORT_URL="shorturl"
LOCATION="eastus2"
RESOURCE_GROUP_NAME="rg-$RG_FRIENDLY_NAME"
STORAGE_ACCOUNT_NAME="stazfuncshorturl"
SKU="Standard_LRS"
KIND="StorageV2"
EVENTHUB_NAMESPACE="evhns-$APP_SHORT_URL-001"
EVENTHUB_NAME="evh-$APP_SHORT_URL"
QUEUE_NAME="stq-$APP_SHORT_URL-usage"

echo RESOURCE_GROUP_NAME: $RESOURCE_GROUP_NAME
echo STORAGE_ACCOUNT_NAME: $STORAGE_ACCOUNT_NAME

echo QUEUE_NAME: $QUEUE_NAME


if [ 1 -eq 0 ]; then
echo "=== Create Storage Account: $STORAGE_ACCOUNT_NAME in ResourceGroup $RESOURCE_GROUP_NAME at Location $LOCATION ==="
az storage account create \
    -n $STORAGE_ACCOUNT_NAME \
    -g $RESOURCE_GROUP_NAME \
    -l $LOCATION \
    --sku $SKU \
    --kind $KIND \
    --access-tier Hot  
az storage queue create --name $QUEUE_NAME --account-name $STORAGE_ACCOUNT_NAME


AZURE_STORAGE_ACCESS_KEY="$(az storage account keys list \
                                --account-name $STORAGE_ACCOUNT_NAME \
                                --resource-group $RESOURCE_GROUP_NAME \
                                --query "[0].value" \
                                --output tsv)"

echo STORAGE_ACCOUNT_NAME: $STORAGE_ACCOUNT_NAME
echo AZURE_STORAGE_ACCESS_KEY: $AZURE_STORAGE_ACCESS_KEY
CONTAINER_NAME="eventcontainer"
az storage container create \
    -n $CONTAINER_NAME \
    --auth-mode login \
    --account-name $STORAGE_ACCOUNT_NAME \
    --account-key $AZURE_STORAGE_ACCESS_KEY  

fi
echo EVENTHUB_NAMESPACE: $EVENTHUB_NAMESPACE
echo EVENTHUB_NAME: $EVENTHUB_NAME

az eventhubs namespace create \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $EVENTHUB_NAMESPACE \
    --location $LOCATION \
    --sku Basic 


az eventhubs eventhub create \
     -g $RESOURCE_GROUP_NAME \
     --namespace-name $EVENTHUB_NAMESPACE \
     --name $EVENTHUB_NAME \
     --message-retention 1 \
     --partition-count 2
 
# az eventhubs namespace authorization-rule keys list --resource-group rg-organics-openhack --namespace-name evhns-organics --name RootManageSharedAccessKey

CONNECTION_STRING=$(az eventhubs namespace authorization-rule keys list --namespace-name $EVENTHUB_NAMESPACE --name RootManageSharedAccessKey --resource-group $RESOURCE_GROUP_NAME --query primaryConnectionString --output tsv)
echo CONNECTION_STRING: $CONNECTION_STRING 
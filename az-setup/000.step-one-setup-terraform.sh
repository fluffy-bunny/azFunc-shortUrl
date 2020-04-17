# https://docs.microsoft.com/en-us/azure/terraform/terraform-backend

die () {
    echo >&2 "$@"
    echo "$ ./az-create-keyvault.sh [name]"
    exit 1
}

echo "Positional Parameters"
echo '$0 = '$0
echo '$1 = '$1
SUBSCRIPTION_NAME=$1 
if [ -z "${SUBSCRIPTION_NAME}" ]; then
    echo "SUBSCRIPTION_NAME is unset or set to the empty string"
    SUBSCRIPTION_NAME="MUSE1-NS01"
fi
echo 'SUBSCRIPTION_NAME = '$SUBSCRIPTION_NAME

az login
echo '----- Current Account -----'
az account show
echo '----- devops -----'
az extension add --name azure-devops
az devops project create --name azfunc_shorturl --org https://dev.azure.com/nlok-general-services    

az devops configure --defaults organization=https://dev.azure.com/nlok-general-services project=azfunc_shorturl
az devops service-endpoint create \
    --service-endpoint-configuration azure_resource_manager_service_connection.json

SUBSCRIPTION_ID="$(az account show -s $SUBSCRIPTION_NAME --query id -o tsv)"
echo 'SUBSCRIPTION_ID = '$SUBSCRIPTION_ID

APP_FRIENDLY_NAME="shorturl"
LOCATION="eastus2"
RESOURCE_GROUP_NAME="rg-terraform-$APP_FRIENDLY_NAME"
CONTAINER_NAME="tstate"
STORAGE_ACCOUNT_NAME="stterraform$APP_FRIENDLY_NAME"
KV_NAME="kv-terraform-$APP_FRIENDLY_NAME"

echo "==== Creating Resource Group: $RESOURCE_GROUP_NAME in Location: $LOCATION"
az group create \
    --name $RESOURCE_GROUP_NAME \
    --location $LOCATION

 
echo "====== Creating KEY VAULT:  $KV_NAME ================="
az keyvault create \
    --location $LOCATION \
    --name $KV_NAME \
    --resource-group $RESOURCE_GROUP_NAME

# Create storage account
az storage account create \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $STORAGE_ACCOUNT_NAME \
    --sku Standard_LRS \
    --encryption-services blob

# Get storage account key
ACCOUNT_KEY=$(az storage account keys list --resource-group $RESOURCE_GROUP_NAME --account-name $STORAGE_ACCOUNT_NAME --query [0].value -o tsv)
 
# Create blob container
az storage container create \
    --name $CONTAINER_NAME \
    --account-name $STORAGE_ACCOUNT_NAME \
    --account-key $ACCOUNT_KEY

echo "storage_account_name: $STORAGE_ACCOUNT_NAME"
echo "container_name: $CONTAINER_NAME"
echo "access_key: $ACCOUNT_KEY"

SECRET_NAME="terraform-backend-key"
VALUE=$ACCOUNT_KEY
az keyvault secret set \
    -n $SECRET_NAME \
    --vault-name $KV_NAME \
    --value "$VALUE"

az keyvault secret show -n $SECRET_NAME --vault-name $KV_NAME
export ARM_ACCESS_KEY=$(az keyvault secret show --name $SECRET_NAME --vault-name $KV_NAME --query value -o tsv)   

die () {
    echo >&2 "$@"
    echo "$ ./az-login-set-subscription.sh [subscription-name(optional)]"
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

SUBSCRIPTION_ID="$(az account show -s $SUBSCRIPTION_NAME --query id -o tsv)"
echo 'SUBSCRIPTION_ID = '$SUBSCRIPTION_ID

az login
az account set --subscription $SUBSCRIPTION_ID
echo '----- Current Account -----'
az account show

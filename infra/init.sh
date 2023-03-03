#!/usr/bin/env sh

tenant_id=`printenv TF_VAR_tenant_id`
subscription_id=`printenv TF_VAR_subscription_id`
client_id=`printenv TF_VAR_client_id`
client_secret=`printenv TF_VAR_client_secret`

terraform init \
    -backend-config="tenant_id=$tenant_id" \
    -backend-config="subscription_id=$subscription_id" \
    -backend-config="client_id=$client_id" \
    -backend-config="client_secret=$client_secret" \
    -backend-config="resource_group_name=elcontoso" \
    -backend-config="storage_account_name=elcontoso"
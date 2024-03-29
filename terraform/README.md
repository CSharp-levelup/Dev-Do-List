# Terraform

Terraform Cloud is used for the remote state backend:
- It will automatically run speculative plans on pull requests if the terraform changes.
- After merging, the plan will need to be manually reviewed and approved before being implemented.

The backend folder is for the API and database infrastructure while the frontend folder is for the web server.

The database is only accessible by the API server and a bastion host in the same VPC.
The bastion host is only accessible through ssm as it is not exposing any ports and has a firewall that blocks all incoming traffic.
To connect to the db through the bastion host:
- Download the aws cli
- Download the session manager plugin for the cli
- Setup sso for the aws account using `aws configure sso`
- Set your profile with an environment variable using `$env:AWS_PROFILE="<profile_name>"`
- Login through sso using `aws sso login`
- Start a port forwarding session on the bastion host to the rds instance using: 
```
aws ssm start-session --target <INSTANCE_ID> \ 
--document-name AWS-StartPortForwardingSessionToRemoteHost \ 
--parameters '{\"localPortNumber\":[\"1433\"],\"portNumber\":[\"1433\"],\"host\":[\"<DB_ENDPOINT>\"]}'
```
- In SQL server management studio, connect with the DB credentials to localhost:1433
- This will let you connect to the RDS instance and run SQL queries
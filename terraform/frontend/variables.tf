variable "common_tags" {
  type        = map(string)
  description = "Common tags applied to all resources"
}

variable "region" {
  type        = string
  description = "The region where the resources will be deployed."
}

variable "vpc_cidr" {
  type        = string
  description = "The CIDR block for the VPC."
}

variable "vpc_az" {
  type        = string
  description = "The availability zone for the VPC."
}

variable "vpc_public_subnet" {
  type        = string
  description = "The public subnet for the VPC."
}

variable "ec2_public_key" {
  type        = string
  description = "The public key to use for the EC2 instance."
}

variable "naming_prefix" {
  type        = string
  description = "The prefix to use for naming resources."
}

variable "client_id" {
  type        = string
  description = "The client ID for oauth environment variable"
  sensitive   = true
}

variable "server_url" {
  type        = string
  description = "The url for the server environment variable"
  sensitive   = true
}
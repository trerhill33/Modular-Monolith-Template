-- Initialize separate databases for each module
-- This script runs automatically when PostgreSQL container starts for the first time

-- Create module databases
CREATE DATABASE orders;
CREATE DATABASE sales;
CREATE DATABASE customer;
CREATE DATABASE inventory;
CREATE DATABASE organization;
CREATE DATABASE sample;

-- Grant privileges (postgres user already has full access)
-- Add any additional setup here as needed

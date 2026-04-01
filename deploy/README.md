# TerminalPrime VPS Deployment

This folder contains a production-ready starter setup for deploying TerminalPrime to a VPS with Docker Compose, Nginx, and GitLab CI.

## Domain layout

- `terminalprime.com` -> Blazor
- `www.terminalprime.com` -> Blazor
- `api-cot.terminalprime.com` -> COT API
- `api-login.terminalprime.com` -> Login API

## Required DNS records

Point these `A` records to your VPS public IP:

- `terminalprime.com`
- `www.terminalprime.com`
- `api-cot.terminalprime.com`
- `api-login.terminalprime.com`

## VPS bootstrap

1. Install Docker and Docker Compose.
2. Create `/opt/terminalprime`.
3. Copy the repository or at least the `deploy/` folder to `/opt/terminalprime`.
4. Copy `deploy/.env.example` to `/opt/terminalprime/.env` and replace every placeholder.
5. Make sure `deploy/nginx/terminalprime.conf` is available at `/opt/terminalprime/deploy/nginx/terminalprime.conf`.

## SSL certificates

The Nginx config expects Let's Encrypt certificates at:

- `/etc/letsencrypt/live/terminalprime.com/`
- `/etc/letsencrypt/live/api-cot.terminalprime.com/`
- `/etc/letsencrypt/live/api-login.terminalprime.com/`

You can issue them with Certbot after DNS is live.

## GitLab variables

Add these masked CI/CD variables in GitLab:

- `VPS_HOST`
- `VPS_USER`
- `VPS_SSH_PRIVATE_KEY`

Your VPS `.env` file should contain runtime secrets such as database connection strings, JWT settings, and Google OAuth credentials.

## CI pipeline

Use `deploy/.gitlab-ci.vps.yml.example` as the base for the VPS pipeline. It builds three images, pushes them to the GitLab registry, and then updates the VPS via SSH.


# HTLLBB


Start with `docker-compose up` in the HTLLBB directory, the application will start listening on http://*:5000. It is intended to run behind a reverse proxy like NGINX or IIS.

### Reminder for deployment

- Configure your SMTP settings in src/appsettings.Production.json

>Google's SMTP server uses config like

```
"Smtp": {
	"Server": "smtp.gmail.com",
	"User": "email@gmail.com",
	"Pass": "password",
	"Port": "587"
}
```

- Edit the MySQL password in docker-compose.yml and src/appsettings.Production.json

- Setup NGINX to serve the backend as a reverse proxy

>A sample NGINX config for reverse proxy could look like:

```
# /etc/nginx/site-enabled/default
location / {
    proxy_pass http://localhost:5000;
    proxy_set_header Host $host;
}

# SignalR stuff
location /chat {
    proxy_read_timeout 86400s;
    proxy_send_timeout 86400s;
    proxy_buffering off;
    proxy_pass http://localhost:5000;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
    proxy_set_header Host $host;
    proxy_cache_bypass $http_upgrade;
}  
```

### For development

- Install dotnetcore on your platform
- Use the following environment variable `export ASPNETCORE_ENVIRONMENT=Development` (~/.bashrc on linux, ~/.profile on OSX)
- Setup a local MySQL and Redis instance, and fill in the required details in src/appsettings.Development.json
- Start the project with `dotnet run` in the src directory (or use the default "play" button in visual studio)


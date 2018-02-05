# HTLLBB


Start with `dotnet run` in the src directory, the application will start listening on http://localhost:5000. It is intended to run behind a reverse proxy like NGINX or IIS.

### Reminder for deployment

- Install docker-ce and docker-compose on your host

- Configure your SMTP settings and your connectionString in appsettings.Production.json.

>Google's SMTP server uses config like

```
"Smtp": {
	"Server": "smtp.gmail.com",
	"User": "email@gmail.com",
	"Pass": "password",
	"Port": "587"
}
```

- Make sure the connectionString matches the mysql docker-compose.yml

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

- the Redis Address correspond to the redis service name in docker-compose.yml

```
"Redis": {
	"Address": "redis"
}
```

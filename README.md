# HTLLBB


Start with `docker-compose up` in the HTLLBB directory, the application will start a couple of containers:

- src: the aspnetcore app, listening on *:5000
- redis: a redis instance
- mysql: a mysql server instance
- nginx: a nginx reverse proxy, listening on *:80

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
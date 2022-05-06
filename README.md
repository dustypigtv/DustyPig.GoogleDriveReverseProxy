[![Auto-Build](https://github.com/dustypigtv/DustyPig.GoogleDriveReverseProxy/actions/workflows/auto_build.yml/badge.svg)](https://github.com/dustypigtv/DustyPig.GoogleDriveReverseProxy/actions/workflows/auto_build.yml)&nbsp;&nbsp;[![Release](https://github.com/dustypigtv/DustyPig.GoogleDriveReverseProxy/actions/workflows/release.yml/badge.svg)](https://github.com/dustypigtv/DustyPig.GoogleDriveReverseProxy/actions/workflows/release.yml)


Simple server, meant to sit behind an nginx reverse proxy, to validate file requests

## Example nginx config:

```
server {

	listen 80;
	listen [::]:80;
	server_name example.com;

	location / {
		auth_request /get_token;
		auth_request_set $token $upstream_http_token;
		resolver 8.8.8.8;
		proxy_buffering off;
		proxy_set_header Authorization 'Bearer $token';
		proxy_pass https://www.googleapis.com/drive/v3/files/$arg_file_id?alt=media;
		proxy_pass_request_body off;
		proxy_set_header Content-Length "";
	}
	
	location = /get_token {
		internal;
		proxy_pass http://localhost:6789/verify;
		proxy_pass_request_body off;
		proxy_set_header Content-Length "";
		proxy_set_header X-Original-URI $request_uri;
    }
}
```
# daikin_net
C# .Net Daikin One Plus Logger
- Polls metrics from Daikin One Plus HVAC system
- Logs metrics to InfluxDB
- Dockerized
- - Configurable via environment variables

## Configuration
- `DAIKIN_USERNAME` - Daikin One login username
- `DAIKIN_PASSWORD` - Daikin One login password
- `INFLUX_AUTH` - Influx DB v1 auth token
- `INFLUX_URL` - InfluxDB db url
- `POLL_INTERVAL` - Interval in seconds to poll Daikin One API. [defualts to 5 seconds]

## Docker
```
docker run -d --name daikin_net --rm \
    -e DAIKIN_USERNAME='user@email.com' \
    -e DAIKIN_PASSWORD='super_secret_pw' \
    -e INFLUX_AUTH='basic_auth_token' \
    -e INFLUX_URL='http://192.168.1.123:8086/write?db=my_db' \
    daikin_net:latest
```

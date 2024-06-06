# daikin_net
C# .Net Daikin One Plus Logger
- Polls metrics from Daikin One Plus HVAC system
- Logs metrics to InfluxDB
- Dockerized
- - Configurable via environment variables

## Configuration
- `DAIKIN_IP` - IP address of Daikin One Plus HVAC system
- `DAIKIN_PORT` - Port of Daikin One Plus HVAC system
- `INFLUXDB_URL` - URL of InfluxDB
- `INFLUXDB_DATABASE` - InfluxDB database name
- `INFLUXDB_USERNAME` - InfluxDB username
- `INFLUXDB_PASSWORD` - InfluxDB password
- `POLL_INTERVAL` - Interval in seconds to poll Daikin One Plus HVAC system

## Docker
```
docker run -d --name daikin_net --rm \
    -e DAIKIN_USERNAME='user@email.com' \
    -e DAIKIN_PASSWORD='super_secret_pw' \
    -e INFLUX_AUTH='basic_auth_token' \
    -e INFLUX_URL='http://192.168.1.123:8086/write?db=my_db' \
    daikin_net:latest
```
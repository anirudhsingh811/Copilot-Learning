# ?? MQTT Broker Configuration Guide

## ?? Table of Contents
1. [Configuration File Structure](#configuration-file-structure)
2. [Common Configuration Options](#common-configuration-options)
3. [How to Modify Configuration](#how-to-modify-configuration)
4. [Security Configuration](#security-configuration)
5. [Advanced Settings](#advanced-settings)
6. [Testing Configuration](#testing-configuration)

---

## ?? Configuration File Structure

### **Current Setup:**

```
infrastructure/
??? docker-compose.yml
??? mosquitto/
    ??? config/
    ?   ??? mosquitto.conf    ? Main configuration file
    ??? data/                 ? Persistent data storage
    ??? log/                  ? Log files
```

### **How It Works:**

The `docker-compose.yml` mounts the local `mosquitto/config` directory into the container:

```yaml
volumes:
  - ./mosquitto/config:/mosquitto/config      # Config files
  - ./mosquitto/data:/mosquitto/data          # Data persistence
  - ./mosquitto/log:/mosquitto/log            # Log files
```

When the container starts, it reads `/mosquitto/config/mosquitto.conf` from inside the container, which is actually your local file.

---

## ?? Common Configuration Options

### **1. Network Settings**

```conf
# MQTT Protocol on port 1883
listener 1883
protocol mqtt

# WebSocket support on port 9001 (for browser clients)
listener 9001
protocol websockets
```

**Use Cases:**
- Port 1883: Standard MQTT clients (your .NET services)
- Port 9001: Web-based MQTT clients, browser applications

### **2. Authentication**

**Current (Development):**
```conf
# Allow any client to connect without credentials
allow_anonymous true
```

**Production (Secure):**
```conf
# Require username/password
allow_anonymous false
password_file /mosquitto/config/passwd
```

### **3. Logging**

```conf
# Log to file and console
log_dest file /mosquitto/log/mosquitto.log
log_dest stdout

# Log levels
log_type error      # Only errors
log_type warning    # Warnings
log_type notice     # Important notices
log_type information # General info
log_type all        # Everything (very verbose)
log_type debug      # Debug messages (extreme verbosity)
```

**Recommendation for Development:**
- Use: `error`, `warning`, `notice`, `information`
- Avoid: `all`, `debug` (too noisy)

### **4. Quality of Service (QoS)**

```conf
# Maximum QoS level supported (0, 1, or 2)
max_qos 2
```

| QoS Level | Guarantee | Use Case |
|-----------|-----------|----------|
| **0** | At most once | Fire-and-forget (fast, no guarantee) |
| **1** | At least once | Acknowledged delivery (may duplicate) |
| **2** | Exactly once | Guaranteed single delivery (slowest) |

**Your POC uses QoS 0** (MQTTnet default).

### **5. Message Persistence**

```conf
# Save messages to disk (survives restarts)
persistence true
persistence_location /mosquitto/data/

# Auto-save interval (seconds)
autosave_interval 1800  # 30 minutes
```

**Benefits:**
- Retained messages survive broker restarts
- Queued messages for offline clients are saved

### **6. Connection Limits**

```conf
# Maximum simultaneous connections (-1 = unlimited)
max_connections -1

# Maximum queued messages per client
max_queued_messages 1000

# Message size limit (bytes, 0 = unlimited)
message_size_limit 0
```

---

## ?? How to Modify Configuration

### **Method 1: Edit Local File (Recommended)**

1. **Open the configuration file:**
   ```
   infrastructure\mosquitto\config\mosquitto.conf
   ```

2. **Make your changes** (see examples below)

3. **Restart the broker:**
   ```powershell
   cd infrastructure
   docker-compose restart mosquitto
   ```

4. **Verify changes:**
   ```powershell
   docker logs mqtt-broker
   ```

### **Method 2: Docker Exec (Temporary Changes)**

```powershell
# Access the container shell
docker exec -it mqtt-broker sh

# Edit config inside container (requires vi/nano)
vi /mosquitto/config/mosquitto.conf

# Exit container
exit

# Restart to apply
docker restart mqtt-broker
```

?? **Warning:** Changes made inside the container are lost if you recreate it.

### **Method 3: Rebuild Container**

If you change `docker-compose.yml`:

```powershell
cd infrastructure
docker-compose down
docker-compose up -d
```

---

## ?? Security Configuration

### **Enable Authentication (Production)**

#### **Step 1: Create Password File**

**On Windows (PowerShell):**
```powershell
# Create password file
docker exec -it mqtt-broker mosquitto_passwd -c /mosquitto/config/passwd admin

# Enter password when prompted
```

**On Linux/Mac:**
```sh
docker exec -it mqtt-broker mosquitto_passwd -c /mosquitto/config/passwd admin
```

#### **Step 2: Add More Users**

```powershell
# Add additional users (without -c flag)
docker exec -it mqtt-broker mosquitto_passwd /mosquitto/config/passwd orderservice
docker exec -it mqtt-broker mosquitto_passwd /mosquitto/config/passwd paymentservice
```

#### **Step 3: Update Configuration**

Edit `infrastructure/mosquitto/config/mosquitto.conf`:

```conf
# Disable anonymous access
allow_anonymous false

# Point to password file
password_file /mosquitto/config/passwd
```

#### **Step 4: Restart Broker**

```powershell
docker-compose restart mosquitto
```

#### **Step 5: Update Your Services**

Modify `src/Shared/MqttClientHelper.cs`:

```csharp
public async Task ConnectAsync()
{
    var options = new MqttClientOptionsBuilder()
        .WithTcpServer(_brokerHost, _brokerPort)
        .WithCredentials("orderservice", "your-password")  // Add this line
        .Build();
    
    await _client.ConnectAsync(options, CancellationToken.None);
}
```

---

## ?? Common Configuration Scenarios

### **Scenario 1: Increase Log Verbosity**

**Problem:** Need more detailed logs for debugging

**Solution:**
```conf
# In mosquitto.conf
log_type all

# Or just debug
log_type debug
```

**Restart:**
```powershell
docker-compose restart mosquitto
```

**View logs:**
```powershell
# Real-time logs
docker logs -f mqtt-broker

# Or check the file
Get-Content infrastructure\mosquitto\log\mosquitto.log -Tail 50 -Wait
```

---

### **Scenario 2: Change MQTT Port**

**Problem:** Port 1883 is already in use

**Solution 1: Change Host Port in docker-compose.yml**

```yaml
ports:
  - "1884:1883"  # Map host 1884 to container 1883
```

**Solution 2: Change Broker Port**

In `mosquitto.conf`:
```conf
listener 1884
protocol mqtt
```

And in `docker-compose.yml`:
```yaml
ports:
  - "1884:1884"
```

**Update your services:**
```csharp
// In MqttClientHelper constructor
public MqttClientHelper(string brokerHost = "localhost", int brokerPort = 1884)
```

---

### **Scenario 3: Disable Persistence (Testing)**

**Problem:** Want fresh broker state on each restart

**Solution:**
```conf
# In mosquitto.conf
persistence false
```

**Or delete data:**
```powershell
Remove-Item -Recurse -Force infrastructure\mosquitto\data\*
```

---

### **Scenario 4: Limit Message Size**

**Problem:** Prevent large messages from overloading broker

**Solution:**
```conf
# Limit to 100KB per message
message_size_limit 102400
```

---

### **Scenario 5: Enable WebSocket for Browser Clients**

**Already enabled in your config!**

```conf
listener 9001
protocol websockets
```

**Test with JavaScript:**
```javascript
// Using mqtt.js in browser
const client = mqtt.connect('ws://localhost:9001');

client.on('connect', () => {
  console.log('Connected via WebSocket');
  client.subscribe('orders/created');
});

client.on('message', (topic, message) => {
  console.log('Received:', message.toString());
});
```

---

## ?? Advanced Settings

### **1. Access Control Lists (ACL)**

Control which clients can publish/subscribe to which topics.

**Create `infrastructure/mosquitto/config/acl`:**

```conf
# Admin has full access
user admin
topic readwrite #

# OrderService can only publish to orders/*
user orderservice
topic write orders/#
topic read orders/#

# PaymentService can read orders and write payments
user paymentservice
topic read orders/created
topic write payments/#
```

**Update `mosquitto.conf`:**
```conf
acl_file /mosquitto/config/acl
```

### **2. Bridge to Another Broker**

Connect your local broker to a cloud MQTT broker.

```conf
# Bridge configuration
connection cloud-bridge
address mqtt.example.com:1883
topic orders/# both 0
username cloud-user
password cloud-pass
```

### **3. Custom Plugin**

Load authentication/authorization plugins.

```conf
plugin /usr/lib/mosquitto_dynamic_security.so
```

---

## ?? Testing Configuration

### **Test 1: Verify Broker is Using Config**

```powershell
# Check if config file is being read
docker logs mqtt-broker | Select-String "mosquitto.conf"
```

**Expected output:**
```
mosquitto version 2.0.x starting
Config loaded from /mosquitto/config/mosquitto.conf
Opening ipv4 listen socket on port 1883.
Opening ipv6 listen socket on port 1883.
Opening websockets listen socket on port 9001.
```

### **Test 2: Check Listener Ports**

```powershell
# From outside container
docker exec mqtt-broker netstat -tuln | Select-String "1883|9001"
```

**Expected:**
```
tcp        0      0 0.0.0.0:1883            0.0.0.0:*               LISTEN
tcp        0      0 0.0.0.0:9001            0.0.0.0:*               LISTEN
```

### **Test 3: Monitor Logs in Real-Time**

```powershell
# Follow logs
docker logs -f mqtt-broker

# Or tail the log file
Get-Content infrastructure\mosquitto\log\mosquitto.log -Tail 20 -Wait
```

### **Test 4: Verify Authentication**

```powershell
# Should succeed with correct credentials
docker exec mqtt-broker mosquitto_sub -h localhost -p 1883 -t test -u admin -P password

# Should fail without credentials (if anonymous disabled)
docker exec mqtt-broker mosquitto_sub -h localhost -p 1883 -t test
```

### **Test 5: Check System Topics**

```powershell
# Subscribe to broker statistics
docker exec mqtt-broker mosquitto_sub -h localhost -t '$SYS/#' -v
```

**You'll see:**
```
$SYS/broker/version mosquitto version 2.0.x
$SYS/broker/clients/connected 3
$SYS/broker/messages/sent 125
$SYS/broker/uptime 3600 seconds
```

---

## ?? Configuration Examples for Different Environments

### **Development (Current)**

```conf
allow_anonymous true
log_type information
persistence true
max_connections -1
```

**Characteristics:**
- ? Easy to use, no auth needed
- ? Good logging
- ?? Not secure

### **Testing/Staging**

```conf
allow_anonymous false
password_file /mosquitto/config/passwd
acl_file /mosquitto/config/acl
log_type warning
persistence true
max_connections 100
```

**Characteristics:**
- ? Authentication enabled
- ? Topic-level access control
- ? Resource limits

### **Production**

```conf
allow_anonymous false
password_file /mosquitto/config/passwd
acl_file /mosquitto/config/acl
log_type error
persistence true
max_connections 1000
autosave_interval 300
max_qos 1
connection_messages false
```

**Characteristics:**
- ? Secure
- ? Minimal logging
- ? Optimized performance
- ? Higher limits

---

## ?? Applying Configuration Changes

### **Quick Reference:**

| Change Type | Action Required |
|-------------|-----------------|
| Edit `mosquitto.conf` | `docker-compose restart mosquitto` |
| Edit `docker-compose.yml` | `docker-compose down && docker-compose up -d` |
| Add password file | `docker-compose restart mosquitto` |
| Add ACL file | `docker-compose restart mosquitto` |

### **Complete Reset:**

```powershell
# Stop and remove everything
cd infrastructure
docker-compose down -v

# Clear data
Remove-Item -Recurse -Force mosquitto\data\*
Remove-Item -Recurse -Force mosquitto\log\*

# Start fresh
docker-compose up -d

# Check logs
docker logs mqtt-broker
```

---

## ?? Troubleshooting

### **Problem: Config changes not taking effect**

```powershell
# Ensure you're restarting, not just reloading
docker-compose restart mosquitto

# Or recreate
docker-compose up -d --force-recreate mosquitto
```

### **Problem: Can't find config file**

```powershell
# Verify file exists
Test-Path infrastructure\mosquitto\config\mosquitto.conf

# Check inside container
docker exec mqtt-broker ls -la /mosquitto/config/
```

### **Problem: Permission denied errors**

```powershell
# On Windows, ensure Docker has access to the folder
# Docker Desktop -> Settings -> Resources -> File Sharing
# Add: C:\Users\INASING7\source\repos\EDA-MQTT-Interview-Prep
```

### **Problem: Broker won't start after config change**

```powershell
# Check for syntax errors
docker logs mqtt-broker

# Common issues:
# - Missing closing quote
# - Invalid option name
# - Wrong file paths
```

**Validate config syntax:**
```powershell
docker exec mqtt-broker mosquitto -c /mosquitto/config/mosquitto.conf -t
```

---

## ?? Configuration Reference

### **Official Documentation:**
- Mosquitto Config: https://mosquitto.org/man/mosquitto-conf-5.html
- Authentication: https://mosquitto.org/documentation/authentication-methods/
- Access Control: https://mosquitto.org/man/mosquitto-conf-5.html#idm120

### **Useful Commands:**

```powershell
# View current config
docker exec mqtt-broker cat /mosquitto/config/mosquitto.conf

# Test config syntax
docker exec mqtt-broker mosquitto -c /mosquitto/config/mosquitto.conf -t

# View broker version
docker exec mqtt-broker mosquitto -h

# Monitor connections
docker exec mqtt-broker mosquitto_sub -t '$SYS/broker/clients/#' -v

# View all system stats
docker exec mqtt-broker mosquitto_sub -t '$SYS/#' -v
```

---

## ? Quick Checklist

After making configuration changes:

- [ ] Edit `mosquitto.conf` file
- [ ] Validate syntax (no typos)
- [ ] Restart broker: `docker-compose restart mosquitto`
- [ ] Check logs: `docker logs mqtt-broker`
- [ ] Test connection from services
- [ ] Verify expected behavior
- [ ] Document changes (if permanent)

---

**Summary:** You now have full control over the MQTT broker configuration. The config file is in `infrastructure/mosquitto/config/mosquitto.conf` and any changes take effect after restarting the container! ??

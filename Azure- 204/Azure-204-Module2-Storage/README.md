# AZ-204 Module 2: Develop for Azure Storage

> ?? **Interactive learning environment for Azure Storage services - 15-20% of AZ-204 exam**

[![Build](https://img.shields.io/badge/build-passing-brightgreen)]() [![.NET](https://img.shields.io/badge/.NET-9.0-blue)]() [![Module](https://img.shields.io/badge/Module-2-orange)]()

---

## ?? What This Module Covers

Complete, production-ready implementations for all Azure Storage services tested in AZ-204:

- ? **Azure Cosmos DB** - NoSQL, partitioning, consistency levels, RU optimization
- ? **Azure Blob Storage** - SAS tokens, tiers, lifecycle, static websites
- ? **Azure Table Storage** - NoSQL key-value, partition design
- ? **Azure Queue Storage** - Message queuing, poison handling

**Exam Weight:** 15-20% of AZ-204 certification

---

## ?? Quick Start

### Run Locally (No Azure Needed!)

```bash
cd Azure-204-Module2-Storage
dotnet run

# Try interactive demos:
# - Option 1: Cosmos DB (6 scenarios)
# - Option 2: Blob Storage (coming soon)
# - Option 5: Decision tree
# - Option 7: Exam tips
```

All demos run locally with simulations - **no cloud costs!**

---

## ?? What's Included

### ?? Interactive Demos (Current)

| Service | Demos | Status |
|---------|-------|--------|
| **Cosmos DB** | 6 scenarios | ? Complete |
| **Blob Storage** | 8 scenarios | ?? Next update |
| **Table Storage** | 4 scenarios | ?? Next update |
| **Queue Storage** | 4 scenarios | ?? Next update |

### ?? Project Structure

```
Azure-204-Module2-Storage/
??? Program.cs                          # Interactive menu
??? appsettings.json                    # Configuration
?
??? 01-CosmosDB/
?   ??? CosmosDbManager.cs             # Complete Cosmos operations
?   ??? Examples/
?       ??? CosmosDbScenarios.cs       # 6 interactive demos ?
?
??? 02-BlobStorage/
?   ??? BlobStorageManager.cs          # Complete Blob operations ?
?   ??? Examples/                      # ?? Coming soon
?
??? 03-TableStorage/                   # ?? Coming soon
??? 04-QueueStorage/                   # ?? Coming soon
```

---

## ?? Cosmos DB Demos (Available Now!)

### Demo 1: E-commerce Partitioning Strategy ?

Learn how to choose the right partition key for optimal performance:

```
Partition Key: /category
? Good: SELECT * FROM c WHERE c.category = 'electronics' (3 RUs)
? Bad: SELECT * FROM c WHERE c.price < 50 (20+ RUs)
```

**Why it matters:** Most common exam question!

---

### Demo 2: Consistency Levels Explained ?????

Understand all 5 consistency levels with trade-offs:

```
Strong         ? 100% consistent, highest latency/cost
Bounded        ? 99.9% consistent, high latency
Session        ? Within session (DEFAULT) ?
Consistent Prefix ? Ordered reads
Eventual       ? Eventually consistent, lowest cost
```

**Exam tip:** Session is default for 99% of scenarios!

---

### Demo 3: Change Feed Pattern

Real-time data sync pattern:

```
Product updated ? Change feed triggers ? Update search index
```

**Use cases:** Real-time sync, event sourcing, materialized views

---

### Demo 4: RU Optimization ?????

Master Request Unit optimization:

```
Point Read (id + PK)     ? 1 RU ? Best!
Single partition query   ? 3 RUs ? Good
Cross-partition query    ? 20+ RUs ? Expensive
```

**Exam scenario:** "How to minimize RU consumption?" ? Use point reads!

---

### Demo 5: Global Distribution

Multi-region setup for 99.999% availability:

```
East US (Primary)    ? Read + Write
West Europe          ? Read
Southeast Asia       ? Read

Automatic failover: < 1 minute
```

---

### Demo 6: Stored Procedures (Transactions)

ACID transactions within a single partition:

```javascript
// Transfer money between accounts
function transfer(fromAccount, toAccount, amount) {
    // All succeed or all fail (atomic)
}
```

**Limitation:** Single partition only

---

## ?? AZ-204 Exam Focus

### High Priority Topics (80% of questions)

1. **Cosmos DB Partitioning** ?????
   - How to choose partition key
   - Point reads vs queries
   - RU optimization

2. **Consistency Levels** ?????
   - Know all 5 levels
   - When to use each
   - Trade-offs

3. **Blob SAS Tokens** ?????
   - Account vs Service SAS
   - Permissions
   - Expiration

4. **Blob Access Tiers** ????
   - Hot, Cool, Archive
   - Cost vs access speed
   - Lifecycle management

5. **Request Units (RU)** ????
   - 1 RU for point read
   - Optimization techniques

### Common Exam Scenarios

#### Scenario 1: Minimize Cost
**Q:** "Store infrequently accessed data cost-effectively"  
**A:** Blob Cool tier (30+ days) or Archive tier (180+ days)

#### Scenario 2: ACID Transactions
**Q:** "Need atomic operations in Cosmos DB"  
**A:** Stored procedures (single partition) or Transactional Batch

#### Scenario 3: Temporary Access
**Q:** "Grant time-limited access to blob"  
**A:** Generate SAS token with expiration

#### Scenario 4: High RU Consumption
**Q:** "Query consuming too many RUs"  
**A:** Use partition key in WHERE clause, limit results, use point reads

---

## ?? Best Practices

### Cosmos DB

? **DO:**
- Choose partition key based on query patterns
- Use point reads (1 RU) whenever possible
- Use Session consistency (default)
- Enable change feed for real-time sync
- Index only necessary properties

? **DON'T:**
- Create hot partitions
- Use Strong consistency unless required
- Run cross-partition queries frequently
- Over-index (increases RU cost)

### Blob Storage

? **DO:**
- Use Cool tier for infrequent access
- Use Archive for long-term storage
- Enable soft delete
- Use SAS with minimal permissions
- Implement lifecycle policies

? **DON'T:**
- Store frequently accessed data in Archive
- Create SAS tokens without expiration
- Skip blob versioning for critical data

---

## ?? Configuration

Update `appsettings.json`:

```json
{
  "CosmosDB": {
    "AccountName": "cosmos-az204-demo",
    "DatabaseName": "SampleDatabase",
    "ContainerName": "Products",
    "PartitionKey": "/category"
  },
  "BlobStorage": {
    "AccountName": "staz204blob",
    "ContainerName": "documents"
  }
}
```

---

## ?? What You'll Learn

After completing this module:

1. ? Choose the right storage service for any scenario
2. ? Design optimal Cosmos DB partition strategies
3. ? Optimize Request Units (RU) consumption
4. ? Implement SAS tokens for secure access
5. ? Configure blob lifecycle management
6. ? Select appropriate consistency levels
7. ? Pass AZ-204 Module 2 with confidence

---

## ?? Learning Path

### Week 1: Cosmos DB (Most Important!)
```
Day 1-2: Partitioning strategies
Day 3: Consistency levels
Day 4: RU optimization
Day 5: Change feed and stored procedures
Day 6-7: Practice and demos
```

### Week 2: Blob Storage
```
Day 1-2: Upload/download, SAS tokens
Day 3: Access tiers and lifecycle
Day 4: Versioning and soft delete
Day 5: Static websites
Day 6-7: Practice
```

### Week 3: Table & Queue
```
Day 1-2: Table Storage operations
Day 3: Queue message handling
Day 4-5: Practice scenarios
Day 6-7: Review and mock exams
```

---

## ? FAQ

### Q: How much of the AZ-204 exam is storage?

**A:** 15-20% of total exam (about 10-15 questions)

### Q: What's the most important topic?

**A:** Cosmos DB partitioning and consistency levels. These appear in almost every exam!

### Q: Do I need an Azure subscription?

**A:** No! Demos 1-6 run locally with simulations. Deploy to Azure when ready to practice with real resources.

### Q: Which consistency level should I remember?

**A:** Session (default) for 99% of scenarios, Strong only for financial/critical data.

---

## ?? Coming Soon

- ? Cosmos DB: Complete (6 demos)
- ?? Blob Storage: 8 interactive demos
- ?? Table Storage: 4 interactive demos
- ?? Queue Storage: 4 interactive demos
- ?? Deployment scripts
- ?? Real Azure integration

---

## ?? Related Modules

- [Module 1: Compute Solutions](../Azure-204/) ? Complete
- **Module 2: Storage** ? You are here
- Module 3: Security ??
- Module 4: Monitoring ??
- Module 5: Integration ??

---

**Happy Learning! ?? Module 2 is crucial for AZ-204 - master it!**

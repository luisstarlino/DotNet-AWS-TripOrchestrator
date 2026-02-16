# ✈️ Trip Orchestrator – Serverless Saga Pattern with AWS Step Functions and .NET 8

![Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=flat)
![.NET](https://img.shields.io/badge/.NET-8-512BD4?style=flat&logo=dotnet&logoColor=white)
![AWS](https://img.shields.io/badge/AWS-Cloud-232F3E?style=flat&logo=amazon-aws&logoColor=white)
![Architecture](https://img.shields.io/badge/Pattern-Saga%20Orchestration-blue?style=flat)
![IaC](https://img.shields.io/badge/IaC-AWS%20CDK-orange?style=flat)

<!-- 
---
## 📖 About the Project

**TripFlow Orchestrator** is a robust **Distributed Transaction System** built with **.NET 8 and AWS Serverless**, designed to demonstrate the implementation of the **Saga Pattern** using Orchestration.

The project solves a classic distributed system problem: **"How to maintain data consistency across microservices when a transaction fails halfway through?"**

It focuses on **resilience and observability**, moving beyond simple CRUDs to show:

- **Distributed Transactions:** Managing state across Flight, Hotel, and Car services.
- **Compensation Logic:** Automatically rolling back changes when a step fails.
- **Infrastructure as Code:** 100% defined in C# using AWS CDK.
- **Chaos Engineering:** Simulating failures to prove system reliability.

> 🧠 "In distributed systems, failure is not an exception; it's a rule. Architecture must embrace it."

---

## 🎯 Architectural Goals

This project was created to address complex backend scenarios such as:

- Handling partial failures in long-running processes.
- Visualizing workflow states with **AWS Step Functions**.
- Implementing **Idempotency** to prevent double-billing.
- Reducing "Cold Starts" with **.NET 8 Native AOT**.

The solution is built upon:

- **Saga Pattern (Orchestration)**
- **Event-Driven Architecture**
- **Serverless Compute (Lambda)**
- **NoSQL Performance (DynamoDB)**
- **Observability (X-Ray)**

---

## 🔄 The Saga Pattern (Orchestration)

Unlike a monolithic transaction (`BEGIN TRAN... COMMIT`), this project orchestrates independent services.

### 🟢 Forward Recovery (Happy Path)

1. **Reserve Flight** (Lambda + DynamoDB)
2. **Reserve Hotel** (Lambda + DynamoDB)
3. **Reserve Car** (Lambda + DynamoDB)
4. ✅ **Trip Confirmed**

### 🔴 Backward Recovery (Compensation/Rollback)

If the **Car Reservation** fails, the Orchestrator triggers the "Undo" logic:

1. ❌ **Car Fails** (Out of stock or error)
2. ↩️ **Cancel Hotel** (Compensating Transaction)
3. ↩️ **Cancel Flight** (Compensating Transaction)
4. 🛑 **Trip Cancelled** (Data consistency restored)

---

## 🧪 Chaos Engineering & Resilience

To demonstrate a senior-level understanding of reliability, this project includes a **"Chaos Toggle"**.

### Key points:

- **Fault Injection:** The API accepts parameters to intentionally fail specific services (e.g., `chaos=car`).
- **Visual Proof:** You can watch the Step Function execution graph divert from the success path to the rollback path in real-time.
- **Idempotency:** Retrying a step doesn't duplicate the reservation.

> We don't just hope it works; we force it to fail to prove it recovers.

---

## 🔌 Infrastructure as Code (CDK)

There is no "ClickOps" here. All infrastructure decisions are code (`.cs`):

- **State Machine Definition:** The workflow logic is defined in C#.
- **Permissions:** Least Privilege access control (IAM) managed via code.
- **Resource Provisioning:** DynamoDB tables and Lambdas are versioned.

This ensures:
- Reproducibility
- No configuration drift
- Professional DevOps practices -->

---

## 🛠️ Technologies Used

| Technology | Description |
|----------|------------|
| **.NET 8** | High-performance runtime (Native AOT capable) |
| **AWS Step Functions** | State Machine Orchestrator |
| **AWS Lambda** | Serverless Compute |
| **Amazon DynamoDB** | NoSQL Database for service state |
| **AWS CDK (C#)** | Infrastructure as Code |
| **AWS X-Ray** | Distributed Tracing |

---

<!-- ## 🚀 How to Run

### Prerequisites
- AWS CLI configured
- Node.js (for CDK)
- .NET 8 SDK

### Deploy Infrastructure
```bash
# Clone the repository
git clone [https://github.com/YOUR-USERNAME/TripFlowOrchestrator.git](https://github.com/YOUR-USERNAME/TripFlowOrchestrator.git)

# Restore dependencies
dotnet restore

# Deploy to AWS
cd infrastructure
cdk bootstrap # (First time only)
cdk deploy -->
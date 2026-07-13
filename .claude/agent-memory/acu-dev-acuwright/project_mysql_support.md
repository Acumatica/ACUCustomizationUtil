---
name: mysql-support-initiative
description: ACU is adding MySQL support (full parity) to its DB layer; the ac.exe MySQL flags are an unverified unknown
metadata:
  type: project
---

ACU is being extended to support **MySQL-backed Acumatica instances**, not just SQL Server.
Decided scope: **full parity** (every DB-touching flow — `src get`, `site install`,
`site update database` — must work on MySQL), an explicit `dbProvider` config switch
(default `mssql`, backward-compatible), and auto-derived connection strings from discrete fields
(`sqlServerName`/`dbName`/new `dbUser`/`dbPassword`) with a full `--dbConnectionString` override.
Recommended `MySqlConnector` as the ADO.NET provider (Dapper-compatible).

**Why:** Acumatica supports MySQL; ACU should manage those instances too. The DB layer today is
hard-wired to SQL Server (`Helpers/DatabaseHelper.cs` is the only place a concrete client is
created; DB connection config lives in the **Site** section, not Src).

**How to apply:** Planning doc at `C:\Users\oleksii.arsirii\.claude\plans\adaptive-jingling-kay.md`.
Two unresolved unknowns needing verification before/while implementing: (1) how `ac.exe` selects
MySQL — no documented `-dbtype` flag; likely `-sw:False` + `-du`/`-dp` (+ `-dboptimize`), verify
with `ac.exe -?` on the target ERP version; (2) whether MySQL needs a `CREATE USER`/`GRANT`
replacement for the SQL-Server-only `UpdateServerLoginDefault` (IIS app-pool Windows login) or a
no-op. Verify both against a real MySQL install.

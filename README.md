# NovaDashWeb

Screenshots for the Nova Dash Web application's front-end UI can be found at

https://raw.githubusercontent.com/gethak/NovaDashWeb/master/NovaAdmin.-.Home.png
and 
https://raw.githubusercontent.com/gethak/NovaDashWeb/master/NovaAdmin.-.Customers.png


##Getting Started.##

Creating and connecting to a Database:

There are two possible ways to set up the database: 
<br /> 1) This is the easier option. Please run the database script called 'Create NovaAdmin RC7.sql' under the AppData folder, and then point the database connection strings in the Nova.Admin project's Web.Config file to use 'NovaAdmin' as the 'Initial Catalog', for example:
    &lt;add name="DefaultConnection" connectionString="Data Source=YourSqlServerInstanceName; Initial Catalog=NovaAdmin; Integrated Security=True" providerName="System.Data.SqlClient" />
    &lt;add name="NovaAdminContext" connectionString="Data Source=YourSqlServerInstanceName; Initial Catalog=NovaAdmin; Integrated Security=True; MultipleActiveResultSets=True;" providerName="System.Data.SqlClient" />
<br /> or 
<br /> 2) If you have SQL Server LocalDB installed, then you will find that the 'Nova.Admin' project's  'App_Data' folder has a database MDF file called 'NovaAdmin.MDF'. When you run the Nova.Admin project, the application will automatically attach this MDF file to the LocalDb SQL instance. Therefore, no additional scripts or any other setup needs to be run. The connection string in the Nova.Admin project's Web.Config folder will then look like: 
&lt;add name="DefaultConnection" connectionString="Data Source=(localdb)\v11.0; AttachDbFilename=|DataDirectory|NovaAdmin.mdf; Initial Catalog=NovaAdmin; Integrated Security=True; MultipleActiveResultSets=True; " providerName="System.Data.SqlClient" />
    &lt;add name="NovaAdminContext" connectionString="Data Source=(localdb)\v11.0; AttachDbFilename=|DataDirectory|NovaAdmin.mdf; Initial Catalog=NovaAdmin; Integrated Security=True; MultipleActiveResultSets=True; " providerName="System.Data.SqlClient" />



The Project includes several infrastructure and '*.Shell' projects which give an indication as to the direction of future refactor to patterns,

A word on the Implementation follows below:


##Architecture:##

The system really composes of two different Usage patterns:
1. Services providing CRUD functions on Customer aggregate-root data, and 
2. Read-only reporting/analytics services on top of the above Customer data. 

If we wanted to over-engineer this, we would place the two under different Domain Contexts with their own DDD implementations. If we were to further exaggerate the use characteristics on these individual services, the CQRS pattern comes to mind as a potential candidate. This is far from a practical solution in real terms for a project of the current nature. I do think it is appropriate to separate the front-end patterns for each of the above usage patterns. In particular, our architecture will comprise of :

1. A plain vanilla MVC front-end providing CRUD features to leverage ASP.NET’s support for Data Annotations (and in turn scaffolding), and
2. A rich Angular/Knockout MVVM LocationAnalytics page that is serviced by a Wep Api.

Both the above will be serviced by a Domain Services API that encapsulates Domain layer logic using Repositories. I realize that this is a potential point of contention, as many feel that Repositories on top of DbContexts providing the Unit-of-Work pattern are redundant, however, I feel repositories do provide better business-context than Query objects. Using the above separation of usage patterns, I’d venture to say that the CRUD features are best left to Repositories whereas the faster evolving Reporting/Analytics aspect belongs in Query (IQueryable) objects, so we will have place for both in our domain Infrastructure.


**So why no MVVM in the current solution?**

With the absence of Html source, I felt the most time efficient manner to get a great UI up and running was to use a html template of some kind. Telerik has an impressive collection of UI patterns in their Kendo UI for MVC toolset. Some of these kendo components had minimal code in them, so the idea was to modify existing html templates to the point where all functionality was working with Nova Dash, and then to refactor the events and multitude of Ajax calls into a ViewModel for use with Angular or Knockout, and wrap these events inside of ViewModel observables and computed observables. 

On a side note, the front end controllers’ implementations were made to accommodate Telerik conventions, such as 
1.	thier DataSourceRequest and DataSourceResponse formats, 
2.	the use of MVC unsupported $inlineCount and $format OData operators, and 
3.	their default use of JSONP



##Some Design Considerations:##

**1. Domain Services layer Implementation - Implementation using Vanilla Web Api 2 or the OData flavour?**

Cons
- Being a Microsoft proprietary standard, it is going to have a tough time finding support in the community. (Major tech cos, including Netflix, are abandoning OData, although these are still early days)
- We expose internal implementation details by tightly coupling a public service definition to the underlying database system. The lack of any clear abstraction between services and underlying data will leave client integrations even more vulnerable to data changes.
- more exposure to abuse using .include(), heavy queries and  in-memory filtering in their respective implementations, etc

Pros
- Great query syntax
- Slips comfortably over the Entity Framework, making it easy to expose data entities as REST resources. 
- Odata v3 has great query limitation features (but once we start limiting, this begs the question, why not just plain vanilla REST then?)
- exposing IQueryable gives the most flexibility and allows for efficient querying as opposed to in-memory filtering, etc, and could reduce the need for making a ton of specific data fetching methods.

I'm still on the fence on this one. OData seems more geared towards browsable public  online data repositories rather than in-house business contexts with high complexity. 


**Change Tracking:**

Some options I considered include
<br/>1.	Use Entity Framework for tracking
Cons:  Only knows abt EF changes, so a tracking system implemented using EF will be oblivious to changes made to a record by Stores Procs, ADO.NET or manual query.
Pros: Quick, easy, provides Audit-log details to the level of which property on a record was changed and by whom.
<br/>2. Use Sql built-in change tracking: (two varieties)
<br/>    - a. Change Data (async, using log files) and 
<br/>    - b. Change Tracking. (sync, bad performance, inconsistent data states, needs Snapshot Isolation level)
<br/>   - both varieties seem oriented towards Sync framework (to data warehouses). 
<br/>   - Change Data is not available for SQL Express. 
<br/>3.	Triggers require us writing logic to derive what was modified, as they simply make copies of entire records

I don’t have a SQL Server professional licence, and since this negates the possibility of using Change Data, I decided to settle for the convenience of the EF approach for now. 
TrackerEnabledDbContext is a tool (although this has had negative Unit testing implications) built on top of EF, that provides attribute and fluent syntax; can choose to [SkipChanges] on properties. It also API to query.

**Domain Design**

I’d also just like to mention that I chose to implement ‘Address’ as an Entity and not as a Value Object only due to Entity Framework’s restrictions on Value Objects not being able to reference other entities. I felt Addresses needed to reference States and Countries for good data integrity and consistency requirements.


**Roadmap:**

Some crucial tools/refactoring I felt I didn’t get to implement yet:
-	AutoMapper for mapping ViewModels to domain DTOs and vice-versa
-	LESS, Typescript
-	Refactoring the DAL code from the CustomerService and LocationAnalyticsService types in the front-end project into the Nova.Admin.Domain.Services api layer.





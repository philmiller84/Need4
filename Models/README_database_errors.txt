EnsureCreated is used to make sure the database exists.

//For testing, the DB is read from the Test/bin/... location <- not always true. could be other bins. or could be specified.


If you need to make a foreign key, will need to seed the child entity first, then use "ForeignTableId" = # as the entry in the parent table


If you need join tables, use the Joins.cs in the API project

HOW TO UPDATE DATABASE? 
 1. Delete Migrations folder, 
 2. Add-Migration init
 3. Update-Database

MISSING DATA?
 Check for an exception in one of the GenericInvokes

DIRECT EDITS TO THE DB NOT WORKING?
 Seed data may be getting re-added. 

MANY TO MANY JOINS NOT WORKING?
 Check the link below. Check that the seed data exists on both sides of the join. Surprisingly, it seems to work as described in this article.
 https://www.thereformedprogrammer.net/updating-many-to-many-relationships-in-entity-framework-core/

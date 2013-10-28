-- Create the database link to CAF
	-- Change the following:
	-- [cafuser]
	-- [userpwd]
	-- [database] (this needs to be in single quotes)
create public database link CAFLINK
	connect to caf identified by userpwd
	using 'w2008x64db';
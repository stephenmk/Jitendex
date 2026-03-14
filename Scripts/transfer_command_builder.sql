-- The following query returns a table of insert command texts
-- for inserting all of the data from an attached schema
-- into identical tables in the main schema.

SELECT 'INSERT INTO '
    || quote('{nameof(' || m.name || ')}')
	|| CHAR(10)
	|| '     ( '
	|| (SELECT group_concat(quote('{nameof(' || m.name || '.' || name || ')}'), CHAR(10) || '     , ') FROM pragma_table_info(m.name))
	|| ')'
	|| CHAR(10)
    || 'SELECT '
	|| (SELECT group_concat('"{nameof(' || m.name || '.' || name || ')}"', CHAR(10) || '     , ') FROM pragma_table_info(m.name))
	|| CHAR(10)
    || '  FROM '
	|| quote('{Schema}')
	|| '.'
	|| quote('{nameof(' || m.name || ')}')
	|| ';'
	|| CHAR(10) AS 'command_texts'
  FROM sqlite_master m
 WHERE m.type = 'table'
   AND m.name NOT LIKE 'sqlite_%';

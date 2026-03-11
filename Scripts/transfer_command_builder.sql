ATTACH DATABASE '/path/to/source/database.db' AS 'source';

-- The following query returns a table of insert command texts
-- for cloning all of the data from the attached database into
-- the corresponding tables in the main database.

WITH common_tables AS (
    SELECT a.name AS table_name
      FROM sqlite_master a
      JOIN 'source'.sqlite_master b
        ON a.name = b.name
     WHERE a.type = 'table'
       AND b.type = 'table'
       AND a.name NOT LIKE 'sqlite_%')
SELECT 'INSERT INTO '
    || quote('{nameof(' || table_name || ')}')
	|| CHAR(10)
	|| '     ( '
	|| (SELECT group_concat(quote('{nameof(' || table_name || '.' || name || ')}'), CHAR(10) || '     , ') FROM pragma_table_info(table_name, 'source'))
	|| ')'
	|| CHAR(10)
    || 'SELECT '
	|| (SELECT group_concat('"{nameof(' || table_name || '.' || name || ')}"', CHAR(10) || '     , ') FROM pragma_table_info(table_name, 'source'))
	|| CHAR(10)
    || '  FROM '
	|| quote('{Schema}')
	|| '.'
	|| quote('{nameof(' || table_name || ')}')
	|| ';'
	|| CHAR(10) AS 'command_texts'
  FROM common_tables;

DETACH DATABASE 'source';

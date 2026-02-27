/*
Copyright (c) 2026 Stephen Kraus
SPDX-License-Identifier: AGPL-3.0-or-later

This file is part of Jitendex.

Jitendex is free software: you can redistribute it and/or modify it under the terms of
the GNU Affero General Public License as published by the Free Software Foundation,
either version 3 of the License or (at your option) any later version.

Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with Jitendex.
If not, see <https://www.gnu.org/licenses/>.
*/

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

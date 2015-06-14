"c:\Program Files\PostgreSQL\9.4\bin\pg_dump.exe" -f cars.sql --username=postgres dbname=gold

"c:\Program Files\PostgreSQL\9.4\bin\psql.exe" -f cars.sql --host=goldilocks.clstkqqph5qy.eu-west-1.rds.amazonaws.com --port=5432 --username=postgres --password --dbname=goldilocks
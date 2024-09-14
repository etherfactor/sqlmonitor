export enum SqlType {
  Unknown = "Unknown",
  SqlServer = "SqlServer",
  MySql = "MySql",
  MariaDb = "MariaDb",
  PostgreSql = "PostgreSql",
}

export function getSqlTypeLabel(value: SqlType) {
  switch (value) {
    case SqlType.Unknown:
      return 'Unknown';

    case SqlType.SqlServer:
      return 'SQL Server';

    case SqlType.MySql:
      return 'MySQL';

    case SqlType.MariaDb:
      return 'MariaDB';

    case SqlType.PostgreSql:
      return 'PostgreSQL';
  }
}

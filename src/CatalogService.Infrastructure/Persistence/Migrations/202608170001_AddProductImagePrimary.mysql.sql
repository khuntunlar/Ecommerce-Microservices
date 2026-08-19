SET @add_is_primary = (
  SELECT IF(
    COUNT(*) = 0,
    'ALTER TABLE `ProductImages` ADD COLUMN `IsPrimary` TINYINT(1) NOT NULL DEFAULT 0 AFTER `SortOrder`',
    'SELECT 1'
  )
  FROM `INFORMATION_SCHEMA`.`COLUMNS`
  WHERE `TABLE_SCHEMA` = DATABASE()
    AND `TABLE_NAME` = 'ProductImages'
    AND `COLUMN_NAME` = 'IsPrimary'
);

PREPARE add_is_primary_statement FROM @add_is_primary;
EXECUTE add_is_primary_statement;
DEALLOCATE PREPARE add_is_primary_statement;

INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('202608170001_AddProductImagePrimary', '9.0.0');

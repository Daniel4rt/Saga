ALTER TABLE `char` ADD `zeny` INT UNSIGNED NOT NULL AFTER `yaw` ,
ADD `save_map` TINYINT UNSIGNED NOT NULL AFTER `zeny` ,
ADD `save_x` FLOAT NOT NULL AFTER `save_map` ,
ADD `save_y` FLOAT NOT NULL AFTER `save_x` ;

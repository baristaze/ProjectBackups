----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE [Type] = 'TF' AND [Name] = 'TokenizeIDs')
BEGIN
	EXEC('DROP FUNCTION [dbo].[TokenizeIDs]');
END
GO

-- here is how to use it: SELECT * FROM TokenizeIds('1,2');
CREATE FUNCTION [dbo].[TokenizeIDs] (@stringToSplit varchar(max))
RETURNS
 @returnList TABLE ([Id] bigint)
AS
BEGIN
	 declare @idAsText varchar(20);
	 declare @id bigint;
	 declare @end int;

	 WHILE (LEN(@stringToSplit) > 0)
	 BEGIN
		set @end  = CHARINDEX(',', @stringToSplit);
		IF (@end = 0)
		BEGIN
			-- not found
			IF(ISNUMERIC(@stringToSplit) = 1)
			BEGIN
				set @id = CONVERT(bigint, @stringToSplit);
				-- add
				INSERT INTO @returnList 
					SELECT @id;
			END
			set @stringToSplit = '';
		END
		ELSE
		BEGIN
			set @idAsText = SUBSTRING(@stringToSplit, 1, @end-1);
			IF(ISNUMERIC(@idAsText) = 1)
			BEGIN
				set @id = CONVERT(bigint, @idAsText);
				INSERT INTO @returnList 
					SELECT @id;
			END
			
			set @stringToSplit = SUBSTRING(@stringToSplit, @end+1, LEN(@stringToSplit)-@end);		
		END
	END
RETURN
END
GO
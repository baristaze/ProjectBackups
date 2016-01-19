----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE [Type] = 'TF' AND [Name] = 'TokenizeGUIDs')
BEGIN
	EXEC('DROP FUNCTION [dbo].[TokenizeGUIDs]');
END
GO

-- here is how to use it: SELECT * FROM TokenizeGUIDs('8087ee5f-bdfc-4206-b4d5-057e281847f2,7a7c7bdb-c072-4b06-a253-7c2047a12d18');
CREATE FUNCTION [dbo].[TokenizeGUIDs] (@stringToSplit varchar(max))
RETURNS
 @returnList TABLE ([Id] uniqueidentifier)
AS
BEGIN
	 declare @idAsText varchar(36);
	 declare @id uniqueidentifier;
	 declare @end int;

	 WHILE (LEN(@stringToSplit) >= 36)
	 BEGIN
		set @end  = CHARINDEX(',', @stringToSplit);
		IF (@end = 0) -- comma not found; this could be first or last one
		BEGIN
			set @id = CONVERT(uniqueidentifier, @stringToSplit);
			INSERT INTO @returnList SELECT @id;
			set @stringToSplit = '';
		END
		ELSE
		BEGIN
			set @idAsText = SUBSTRING(@stringToSplit, 1, @end-1);
			set @id = CONVERT(uniqueidentifier, @idAsText);
			INSERT INTO @returnList SELECT @id;			
			set @stringToSplit = SUBSTRING(@stringToSplit, @end+1, LEN(@stringToSplit)-@end);		
		END
	END
RETURN
END
GO
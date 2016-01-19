----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE [Type] = 'TF' AND [Name] = 'TokenizeStrings')
BEGIN
	EXEC('DROP FUNCTION [dbo].[TokenizeStrings]');
END
GO

-- here is how to use it: SELECT * FROM TokenizeIds('abc,defg and something else');
CREATE FUNCTION [dbo].[TokenizeStrings] (@stringToSplit varchar(max))
RETURNS
 @returnList TABLE ([Text] nvarchar(max))
AS
BEGIN
	 declare @token nvarchar(max);
	 declare @end int;

	 set @stringToSplit = LTRIM(RTRIM(@stringToSplit));

	 WHILE (LEN(@stringToSplit) > 0)
	 BEGIN
		set @stringToSplit = LTRIM(RTRIM(@stringToSplit));
		set @end  = CHARINDEX(',', @stringToSplit);
		IF (@end = 0)
		BEGIN
			-- not found... i.e. it is a single text with no comma
			-- insert it to the return table
			INSERT INTO @returnList 
					SELECT @stringToSplit;

			-- set the input to empty string so that we stop looping
			set @stringToSplit = '';
		END
		ELSE
		BEGIN
			set @token = SUBSTRING(@stringToSplit, 1, @end-1);
			set @token = LTRIM(RTRIM(@token));
			IF(LEN(@token) > 0)
			BEGIN
				INSERT INTO @returnList 
					SELECT @token;
			END
			
			set @stringToSplit = SUBSTRING(@stringToSplit, @end+1, LEN(@stringToSplit)-@end);		
		END
	END
RETURN
END
GO
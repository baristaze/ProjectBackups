
-- ********************************************************************************** --
-- IT IS DANGEROUS TO RUN THIS IN PRODUCTION ENVIRONMENT SINCE @@IDENTITY is not safe
-- ********************************************************************************** --

/*
--declare @testUserId bigint;
--EXEC [dbo].[CreateNewUser] NULL
--set @testUserId = @@IDENTITY; -- don't confuse with SCOPE_IDENTITY
----set @testUserId = 1;
--SELECT * From [User];

--declare @newTopicId bigint;
--EXEC [dbo].[CreateNewTopic] N'The Big Bang Theory', @testUserId;
--set @newTopicId = @@IDENTITY; -- don't confuse with SCOPE_IDENTITY
--SELECT * FROM [dbo].[Topic];

EXEC [dbo].[CreateNewEntry] 5, 'other entry', 1, 2;
select * from [dbo].[Entry];

EXEC [dbo].[UpdateEntry] 1, 'updated first entry x', 1, 1, 1;
EXEC [dbo].[UpdateEntry] 2, 'admin updated entry', 1, 0, -1;

EXEC [dbo].[UpdatEntryStatus] 1, 4, 0, -1;

EXEC [dbo].[DeleteEntry] 1, 1, 1

select * from [dbo].[Vote];
EXEC [dbo].[VoteForEntry] 7, 1, 1
EXEC [dbo].[VoteForEntry] 8, 1, -3
EXEC [dbo].[VoteForEntry] 7, 2, 1
EXEC [dbo].[VoteForEntry] 7, 2, 1

select * from [dbo].[Entry];
select * FROM [dbo].[ReactionType];
select * FROM [dbo].[Reaction];

EXEC [dbo].[ReactToEntry] 7, 1, 2;
EXEC [dbo].[ReactToEntry] 7, 1, 4;
EXEC [dbo].[ReactToEntry] 7, 2, 4;
EXEC [dbo].[ReactToEntry] 7, 1, 16;
EXEC [dbo].[ReactToEntry] 8, 2, 4;

--@TopicId,@TopX,@CheckStatus,@Status,@FromOldToNew,@CurrentEntityId
EXEC [dbo].[GetEntryIDs] 5, 10, 1, 0, 0, 9

--@TopicId,@CurrentUserId,@TopX,@CheckStatus,@Status,@FromOldToNew,@CurrentEntityId
EXEC [dbo].[GetEntries] 5, 1, 10, 1, 0, 1, 0
*/

-- SELECT newid()

EXEC [dbo].[CreateNewImageMetaFile] 
 '66E6274F-9A46-40B7-82BA-B07DF1E5DDA8'
,2
,0
,0
,'image/jpeg'
,1234
,'foo.jpg'
,'http://azure.com/foo.jpg'
,600
,800;

select * from [dbo].[Entry]; -- 7, 8, 9 created by 1, 1, 2
select * from [dbo].[ImageMetaFile]; -- 3, 5, 7 created by 1, 1, 2

EXEC [dbo].[DeleteImageMetaFile] 2, 0, 2;

EXEC [dbo].[GetUnAssociatedImageIDs] 1;
EXEC [dbo].[GetUnAssociatedImageIDs] 2;

-- @Id,@AssetId,@AssetType,@CheckUser,@UserId
EXECUTE [dbo].[AssociateImageToEntry] 7, 8, 1, 0, -1
 

-- ********************************************************************************** --
-- IT IS DANGEROUS TO RUN THIS IN PRODUCTION ENVIRONMENT SINCE @@IDENTITY is not safe
-- ********************************************************************************** --

--EXEC [dbo].[CreateNewUser] NULL
--SELECT * From [User]

/*
declare @testUserId bigint;
set @testUserId = 1;

declare @newTopicId bigint;
EXEC [dbo].[CreateNewTopic] N'The Big Bang Theory', @testUserId;
set @newTopicId = @@IDENTITY; -- don't confuse with SCOPE_IDENTITY
print 'New Topic ID is: ' + CAST(@newTopicId AS varchar(20));
SELECT * FROM [dbo].[Topic];

WAITFOR DELAY '00:00:01';
EXEC [dbo].[UpdateTopic] @newTopicId, N'Einstein', 1, @testUserId;
SELECT * FROM [dbo].[Topic];

EXEC [dbo].[UpdateTopicStatus] @newTopicId, 5, 1, @testUserId;
SELECT * FROM [dbo].[Topic];

INSERT INTO [dbo].[Category] VALUES('General');
INSERT INTO [dbo].[Category] VALUES('Definition');
INSERT INTO [dbo].[Category] VALUES('News');
INSERT INTO [dbo].[Category] VALUES('Pop Culture');
SELECT * FROM [dbo].[Category];

INSERT INTO [TopicCategory]([CategoryId],[TopicId]) VALUES (1, 1);
SELECT * FROM [TopicCategory];
GO

EXEC [dbo].[UpdateTopicStatus] 1, 0, 1, 1;
EXEC [dbo].[GetTopic] 1, 0, 0;

EXEC [dbo].[GetTopic] @newTopicId, 1, 0;

EXEC [dbo].[DeleteTopicIfNoEntry] 1, 1, 1;

EXEC [dbo].[DeleteTopicIfNoEntry] @newTopicId, 1, 1;
SELECT * FROM [dbo].[Topic];

declare @t1 bigint;
EXEC [dbo].[CreateNewTopic] N'Foo', 1;
set @t1 = @@IDENTITY; 

declare @t2 bigint;
EXEC [dbo].[CreateNewTopic] N'Bar', 1;
set @t2 = @@IDENTITY; 

declare @t3 bigint;
EXEC [dbo].[CreateNewTopic] N'Yooo', 1;
set @t3 = @@IDENTITY; 

EXEC [dbo].[RelateTopics] @t1, @t2, 5;
EXEC [dbo].[RelateTopics] @t1, @t2, 10;
EXEC [dbo].[RelateTopics] @t1, @t3, 20;
EXEC [dbo].[RelateTopics] @t2, @t3, 30;
EXEC [dbo].[RelateTopics] @t1, 88888888, 100;
EXEC [dbo].[RelateTopics] @t2, 99999999, 200;
EXEC [dbo].[RelateTopics] @t3, 55555555, 300;
SELECT * FROM [dbo].[RelatedTopic];

EXEC [dbo].[GetRelatedTopics] 2, 1, 0;
EXEC [dbo].[GetRelatedTopics] 3, 1, 0;
EXEC [dbo].[GetRelatedTopics] 4, 1, 0;
EXEC [dbo].[GetRelatedTopics] 88888888, 1, 0;

EXEC [dbo].[DeleteTopicIfNoEntry] 2, 1, 1;
EXEC [dbo].[DeleteTopicIfNoEntry] 3, 1, 1;
EXEC [dbo].[DeleteTopicIfNoEntry] 4, 1, 1;

SELECT * FROM [dbo].[Topic];
SELECT * FROM [dbo].[RelatedTopic];
*/
/************************************************************************************************************/
/************************************************************************************************************/
----------------------------------
--------- prerequisites ----------
----------------------------------
-- delete from [dbo].[SplitSection] where [Id] IN (3,4,5,6);
INSERT INTO [dbo].[SplitSection] ([Id], [Name]) VALUES (1, 'cover');
INSERT INTO [dbo].[SplitSection] ([Id], [Name]) VALUES (2, 'entries');
INSERT INTO [dbo].[SplitSection] ([Id], [Name]) VALUES (3, 'signup-encourage-pg');
INSERT INTO [dbo].[SplitSection] ([Id], [Name]) VALUES (4, 'signup-encourage-title');
INSERT INTO [dbo].[SplitSection] ([Id], [Name]) VALUES (5, 'compare-entries');
INSERT INTO [dbo].[SplitSection] ([Id], [Name]) VALUES (6, 'select-friends');
-- select * from [dbo].[SplitSection] ORDER BY [Id]
GO

-- delete from [dbo].[SplitVariation] where [SectionId] = 3;
INSERT INTO [SplitVariation]([SectionId], [Id], [IsDefault], [Description]) VALUES(1, 1, 1, 'Social Network Theme');
INSERT INTO [SplitVariation]([SectionId], [Id], [IsDefault], [Description]) VALUES(1, 2, 0, 'Godfather Theme');
INSERT INTO [SplitVariation]([SectionId], [Id], [IsDefault], [Description]) VALUES(1, 3, 0, 'Social News Theme');

INSERT INTO [SplitVariation]([SectionId], [Id], [IsDefault], [Description]) VALUES(3, 1, 1, 'With PG-13');
INSERT INTO [SplitVariation]([SectionId], [Id], [IsDefault], [Description]) VALUES(3, 2, 0, 'Without PG-13');

INSERT INTO [SplitVariation]([SectionId], [Id], [IsDefault], [Description]) VALUES(4, 1, 1, 'Friends Talking');
INSERT INTO [SplitVariation]([SectionId], [Id], [IsDefault], [Description]) VALUES(4, 2, 0, 'Friends Saying');
INSERT INTO [SplitVariation]([SectionId], [Id], [IsDefault], [Description]) VALUES(4, 3, 0, 'Friends Sharing');
-- select * from [dbo].[SplitVariation] ORDER BY [SectionId], [Id]
GO

/************************************************************************************************************/
/************************************************************************************************************/
----------------------------------
--------- inserts ----------------
----------------------------------
-- delete from [dbo].[SplitProperty]

/* CSS */
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Background Image', 'split-landing', 'background-image:url({0});', '__APP_PATH__/Images/bkg.png';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Background Tint Color', 'split-left-inner', 'background-color:{0};', '#55a4f2';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Background Tint Opacity', 'split-left-inner', NULL,
	'opacity: 0.3; -moz-opacity:0.3; -khtml-opacity: 0.3; filter: alpha(opacity=30); -ms-filter:''progid:DXImageTransform.Microsoft.Alpha(Opacity=30)'';';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Brand Text Color', 'split-logo-wrp', 'color:{0};', 'White';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Brand Background Color', 'split-logo-wrp', 'background-color:{0};', '#be004c';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Brand Border Color', 'split-logo-wrp', 'border:1px solid {0};', 'Black';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Idea Text Color', 'split-idea-wrp', 'color:{0}', 'White';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Idea Background Color', 'split-idea-wrp', 'background-color:{0};', 'Black';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Idea Background Opacity', 'split-idea-wrp', NULL,
	'opacity: 0.75; -moz-opacity:0.75; -khtml-opacity: 0.75; filter: alpha(opacity=75); -ms-filter:''progid:DXImageTransform.Microsoft.Alpha(Opacity=75)'';';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Idea Border Color', 'split-idea-wrp', 'border:1px solid {0};', 'White';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Facebook Background Color', 'split-facebook-signup', 'background-color:{0};', '#3b5998';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Facebook Border Color', 'split-facebook-signup', 'border:1px solid {0};', 'Black';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - WaitList Message Alignment', 'split-waiting-list', 'text-align:{0};', 'justify';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Invite Background Color', 'split-invite', 'background-color:{0};', '#3b5998';
EXEC [dbo].[InsertCssSplitters] 1, 1, 'Cover Page - Invite Border Color', 'split-invite', 'border:1px solid {0};', 'Black';

/* TEXT */
EXEC [dbo].[InsertTextSplitters] 1, 1, 'Cover Page - Brand Text', 1, 'logoTextCtrl', 'vizibuzz';
EXEC [dbo].[InsertTextSplitters] 1, 1, 'Cover Page - Idea Text', 1, 'ideaTextCtrl', 'Inspire Creativity! <br/>vizibuzz connects you to the world.';
EXEC [dbo].[InsertTextSplitters] 1, 1, 'Cover Page - Join Text', 1, 'joinTextCtrl', 'Please join our private beta!';
EXEC [dbo].[InsertTextSplitters] 1, 1, 'Cover Page - Facebook Text', 1, 'signupTextCtrl', 'Sign up with Facebook';

declare @desc1 nvarchar(max);
set @desc1 = '' +
	'Thank you! We have enrolled you into our waiting list. ' + 
	'We will notify you once we are ready to onboard more private beta users. ' +
	'Please help us with spreading the word by inviting your friends! ' +
	'Your place in the waiting list will be <span class=''emphasize''>' +
	'bumped up as you <a href=''javascript:void(0);'' class=''inviteLink''>invite</a> more</span>!';

EXEC [dbo].[InsertTextSplitters] 1, 1, 'Cover Page - WaitList Text', 1, 'waitListTextCtrl', @desc1;

/*
EXEC [dbo].[UpdateSplitter2] 1, 1, 'Cover Page - Facebook Text', 'Connect with Facebook';
EXEC [dbo].[UpdateSplitter2] 1, 1, 'Cover Page - Join Text', 'Connect with Facebook for a better experience!';
EXEC [dbo].[UpdateSplitter2] 1, 1, 'Cover Page - WaitList Text', 'Please help us to spread the word by inviting your friends!';
EXEC [dbo].[UpdateSplitter2] 1, 1, 'Cover Page - WaitList Message Alignment', 'center';
*/

-- select * from [dbo].[SplitProperty];
-- EXEC [dbo].[GetCssSplitters] 1, 1, 0;
-- EXEC [dbo].[GetCssSplitters] 1, 1, 1;
-- EXEC [dbo].[GetTextSplitters] 1, 1, 0;
-- EXEC [dbo].[GetTextSplitters] 1, 1, 1;

----------------------------------
----------- copy -----------------
----------------------------------
EXEC [dbo].[CopySplitters] 1, 1, 2;

-- select * from [dbo].[SplitProperty] WHERE [VariationId] = 2;
-- EXEC [dbo].[GetCssSplitters] 1, 2, 0;
-- EXEC [dbo].[GetCssSplitters] 1, 2, 1;
-- EXEC [dbo].[GetTextSplitters] 1, 2, 0;
-- EXEC [dbo].[GetTextSplitters] 1, 2, 1;

----------------------------------
----------- update ---------------
----------------------------------

-- css
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Background Image', '__APP_PATH__/Images/bkg2.jpg';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Background Tint Color', 'Transparent';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Brand Text Color', '#555';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Brand Background Color', 'Orange';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Brand Border Color', 'White';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Idea Text Color', '#ff8e07';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Idea Background Color', '#333';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Idea Border Color', 'Gray';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Facebook Background Color', 'Orange';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - WaitList Message Alignment', 'center';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Invite Background Color', '#dc2230';

-- texts
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Brand Text', 'Movilex';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Idea Text', 'I''m gonna make you an offer you won''t refuse';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Join Text', 'Join the family!';
EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - Facebook Text', 'Show your face';

declare @desc2 nvarchar(max);
set @desc2 = 'You gotta <span class=''emphasize''><a href=''javascript:void(0);'' ' +
			 'class=''inviteLink''>invite</a> more</span> to prove your loyalty!';

EXEC [dbo].[UpdateSplitter2] 1, 2, 'Cover Page - WaitList Text', @desc2;

-- select * from [dbo].[SplitProperty] WHERE [VariationId] = 2;
-- EXEC [dbo].[GetCssSplitters] 1, 2, 0;
-- EXEC [dbo].[GetCssSplitters] 1, 2, 1;
-- EXEC [dbo].[GetTextSplitters] 1, 2, 0;
-- EXEC [dbo].[GetTextSplitters] 1, 2, 1;

----------------------------------
----------- copy -----------------
----------------------------------
EXEC [dbo].[CopySplitters] 1, 1, 3;

-- select * from [dbo].[SplitProperty] WHERE [VariationId] = 3;
-- EXEC [dbo].[GetCssSplitters] 1, 3, 0;
-- EXEC [dbo].[GetCssSplitters] 1, 3, 1;
-- EXEC [dbo].[GetTextSplitters] 1, 3, 0;
-- EXEC [dbo].[GetTextSplitters] 1, 3, 1;

----------------------------------
----------- update ---------------
----------------------------------

-- css
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Background Image', '__APP_PATH__/Images/bkg3.jpg';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Background Tint Color', 'White';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Background Tint Opacity', 
	'opacity: 0.5; -moz-opacity:0.5; -khtml-opacity: 0.5; filter: alpha(opacity=50); -ms-filter:''progid:DXImageTransform.Microsoft.Alpha(Opacity=50)'';';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Brand Text Color', 'Black';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Brand Background Color', 'Green';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Brand Border Color', 'Black';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Idea Text Color', 'Black';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Idea Background Color', 'Green';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Idea Border Color', 'Black';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Idea Background Opacity', 
	'opacity: 1.0; -moz-opacity:1.0; -khtml-opacity: 1.0; filter: alpha(opacity=100); -ms-filter:''progid:DXImageTransform.Microsoft.Alpha(Opacity=100)'';';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Facebook Background Color', '#278fc4';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - WaitList Message Alignment', 'justify';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Invite Background Color', 'Green';

-- texts
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Brand Text', 'News Chimp';
EXEC [dbo].[UpdateSplitter2] 1, 3, 'Cover Page - Idea Text', 'We connect people in realtime with the hottest content of the moment';

-- select * from [dbo].[SplitProperty] WHERE [VariationId] = 3;
-- EXEC [dbo].[GetCssSplitters] 1, 3, 0;
-- EXEC [dbo].[GetCssSplitters] 1, 3, 1;
-- EXEC [dbo].[GetTextSplitters] 1, 3, 0;
-- EXEC [dbo].[GetTextSplitters] 1, 3, 1;

----------------------------------
----------- enable ---------------
----------------------------------
-- css
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Background Image', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Background Tint Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Background Tint Opacity', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Brand Background Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Brand Border Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Brand Text Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Facebook Background Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Facebook Border Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Idea Background Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Idea Background Opacity', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Idea Border Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Idea Text Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Invite Background Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Invite Border Color', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - WaitList Message Alignment', 1;

-- texts
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Brand Text', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Facebook Text', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Idea Text', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - Join Text', 1;
EXEC [dbo].[ChangeSplitterStatus2] 1, 'Cover Page - WaitList Text', 1;

EXEC [dbo].[GetCssSplitters] 1, 1, 0;
EXEC [dbo].[GetCssSplitters] 1, 2, 0;
EXEC [dbo].[GetCssSplitters] 1, 3, 0;
EXEC [dbo].[GetTextSplitters] 1, 1, 0;
EXEC [dbo].[GetTextSplitters] 1, 2, 0;
EXEC [dbo].[GetTextSplitters] 1, 3, 0;


/************************************************************************************************************/
/************************************************************************************************************/

EXEC [dbo].[InsertCssSplitters] 3, 1, 'Encourage Signup - PG13 Image', 'split-pg13-img', 'display:{0};', 'inline-block';
EXEC [dbo].[InsertCssSplitters] 3, 1, 'Encourage Signup - PG13 Text Left Padding', 'split-pg13-txt-wrp', 'padding-left:{0}px;', '10';
EXEC [dbo].[InsertCssSplitters] 3, 1, 'Encourage Signup - PG13 Text Font Size', 'split-pg13-txt-wrp', 'font-size:{0}px;', '12';
EXEC [dbo].[InsertCssSplitters] 3, 1, 'Encourage Signup - PG13 FB Connect Left Margin', 'signup-modal-install-wrp-left', 'width:{0}px;', '121';
EXEC [dbo].[InsertTextSplitters] 3, 1, 'Encourage Signup - PG13 Text Desktop', 0, '.split-pg-13-warning-desktop', 'Are you over 13?<br/>This topic maynot be suitable for children. Please login to verify your age.';
EXEC [dbo].[InsertTextSplitters] 3, 1, 'Encourage Signup - PG13 Text Mobile', 0, '.split-pg-13-warning-mobile', 'Are you over 13? Login to verify.';

----------------------------------
----------- copy -----------------
----------------------------------
EXEC [dbo].[CopySplitters] 3, 1, 2;

-- select * from [dbo].[SplitProperty] WHERE [VariationId] = 3;
-- EXEC [dbo].[GetCssSplitters] 3, 2, 0;
-- EXEC [dbo].[GetCssSplitters] 3, 2, 1;
-- EXEC [dbo].[GetTextSplitters] 3, 2, 0;
-- EXEC [dbo].[GetTextSplitters] 3, 2, 1;

EXEC [dbo].[UpdateSplitter2] 3, 2, 'Encourage Signup - PG13 Image', 'none';
EXEC [dbo].[UpdateSplitter2] 3, 2, 'Encourage Signup - PG13 Text Left Padding', '0';
EXEC [dbo].[UpdateSplitter2] 3, 2, 'Encourage Signup - PG13 Text Font Size', '14';
EXEC [dbo].[UpdateSplitter2] 3, 2, 'Encourage Signup - PG13 FB Connect Left Margin', '60';
EXEC [dbo].[UpdateSplitter2] 3, 2, 'Encourage Signup - PG13 Text Desktop', 'Please login to verify that you are a friend of the sender so that you can see the shared item.';
EXEC [dbo].[UpdateSplitter2] 3, 2, 'Encourage Signup - PG13 Text Mobile', 'Login to see the shared item.';

EXEC [dbo].[ChangeSplitterStatus2] 3, 'Encourage Signup - PG13 Image', 1;
EXEC [dbo].[ChangeSplitterStatus2] 3, 'Encourage Signup - PG13 Text Left Padding', 1;
EXEC [dbo].[ChangeSplitterStatus2] 3, 'Encourage Signup - PG13 Text Font Size', 1;
EXEC [dbo].[ChangeSplitterStatus2] 3, 'Encourage Signup - PG13 FB Connect Left Margin', 1;
EXEC [dbo].[ChangeSplitterStatus2] 3, 'Encourage Signup - PG13 Text Desktop', 1;
EXEC [dbo].[ChangeSplitterStatus2] 3, 'Encourage Signup - PG13 Text Mobile', 1;

/************************************************************************************************************/
/************************************************************************************************************/

EXEC [dbo].[InsertTextSplitters] 4, 1, 'Encourage Signup - Title', 0, '.split-intall-dialog-curiosity', 'What are your friends talking about??';
EXEC [dbo].[InsertTextSplitters] 4, 2, 'Encourage Signup - Title', 0, '.split-intall-dialog-curiosity', 'What are your friends saying??';
EXEC [dbo].[InsertTextSplitters] 4, 3, 'Encourage Signup - Title', 0, '.split-intall-dialog-curiosity', 'What have your friends shared??';

EXEC [dbo].[ChangeSplitterStatus2] 4, 'Encourage Signup - Title', 1;

/************************************************************************************************************/
/************************************************************************************************************/
-- Split 0: All Defaults
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (0, 'All Defaults');

-- Split 1: Social Network Theme + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (1, 'Social Network Theme + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (1, 1, 1);

-- Split 2: GodFather Theme + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (2, 'GodFather Theme + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (2, 1, 2);

-- Split 3: Social News Theme + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (3, 'Social News Theme + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (3, 1, 3);

------------------------------------------------------------------------------

-- Split 11: PG-13 & Talking + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (11, 'PG-13 & Talking + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (11, 3, 1);
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (11, 4, 1);

-- Split 12: PG-13, Saying + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (12, 'PG-13 & Saying + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (12, 3, 1);
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (12, 4, 2);

-- Split 13: PG-13, Sharing + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (13, 'PG-13 & Sharing + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (13, 3, 1);
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (13, 4, 3);

-- Split 14: No-PG-13, Talking + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (14, 'No-PG-13 & Talking + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (14, 3, 2);
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (14, 4, 1);

-- Split 15: No-PG-13, Saying + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (15, 'No-PG-13 & Saying + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (15, 3, 2);
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (15, 4, 2);

-- Split 16: No-PG-13, Sharing + default for rest --
INSERT INTO [dbo].[Split] ([Id], [Description]) VALUES (16, 'No-PG-13 & Sharing + default for rest');
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (16, 3, 2);
INSERT INTO [dbo].[SplitBag]([SplitId], [SectionId], [VariationId]) VALUES (16, 4, 3);

------------------------------------------------------------------------------
-- SELECT * FROM [dbo].[SplitBag]
/*
EXEC [dbo].[GetSplitInfo] 0, 0, '1,2,3,4,5,6'
EXEC [dbo].[GetSplitInfo] 1, 0, '1,2,3,4,5,6'
EXEC [dbo].[GetSplitInfo] 0, 888, '1,2,3,4,5,6'
EXEC [dbo].[GetSplitInfo] 1, 888, '1,2,3,4,5,6'

EXEC [dbo].[GetSplitInfo] 0, 1, '1'
EXEC [dbo].[GetSplitInfo] 1, 1, '1'
EXEC [dbo].[GetSplitInfo] 0, 2, '1'
EXEC [dbo].[GetSplitInfo] 1, 2, '1'
EXEC [dbo].[GetSplitInfo] 0, 3, '1'
EXEC [dbo].[GetSplitInfo] 1, 3, '1'

EXEC [dbo].[GetSplitInfo] 0, 11, '1'
EXEC [dbo].[GetSplitInfo] 1, 11, '1'
EXEC [dbo].[GetSplitInfo] 0, 888, '1'
EXEC [dbo].[GetSplitInfo] 1, 888, '1'

EXEC [dbo].[GetSplitInfo] 0, 11, '3,4'
EXEC [dbo].[GetSplitInfo] 1, 11, '3,4'
EXEC [dbo].[GetSplitInfo] 0, 12, '3,4'
EXEC [dbo].[GetSplitInfo] 1, 12, '3,4'
EXEC [dbo].[GetSplitInfo] 0, 13, '3,4'
EXEC [dbo].[GetSplitInfo] 1, 13, '3,4'
EXEC [dbo].[GetSplitInfo] 0, 14, '3,4'
EXEC [dbo].[GetSplitInfo] 1, 14, '3,4'
EXEC [dbo].[GetSplitInfo] 0, 15, '3,4'
EXEC [dbo].[GetSplitInfo] 1, 15, '3,4'
EXEC [dbo].[GetSplitInfo] 0, 16, '3,4'
EXEC [dbo].[GetSplitInfo] 1, 16, '3,4'

EXEC [dbo].[GetSplitInfo] 0, 2, '3,4'
EXEC [dbo].[GetSplitInfo] 1, 2, '3,4'
EXEC [dbo].[GetSplitInfo] 0, 888, '3,4'
EXEC [dbo].[GetSplitInfo] 1, 888, '3,4'

*/
GO



select * from [User] WHERE [Id] = '32141998-80F2-4659-83E9-B112C3F649EF';
select * from [Preference] WHERE [UserId] = '32141998-80F2-4659-83E9-B112C3F649EF';
select * from [FacebookUser] WHERE [UserId] = '32141998-80F2-4659-83E9-B112C3F649EF';
select * from [WorkHistory]
select * from [EducationHistory]


/*
DROP TABLE [ImageMetaFile];
DROP TABLE [EducationHistory];
DROP TABLE [WorkHistory];
DROP TABLE [Preference];
DROP TABLE [FacebookFriend];
DROP TABLE [FacebookUser];
DROP TABLE [User];
DROP TABLE [Split];
*/

INSERT INTO [User] VALUES ('32141998-80F2-4659-83E9-B112C3F649EF', 1, 0, 0, '?????', '????', GETUTCDATE(), GETUTCDATE());
INSERT INTO [Preference] VALUES ('32141998-80F2-4659-83E9-B112C3F649EF', 2, GETUTCDATE());

INSERT INTO [FacebookUser] VALUES ('32141998-80F2-4659-83E9-B112C3F649EF', 762118888, N'Barış', N'Taze', N'Barış Taze', 'baristaze@hotmail.com', '1979-02-10 00:00:00.000',
1, 2, 'Married', 'Software Engineer', 4, 'https://www.facebook.com/baris.taze', 'baris.taze', 'Redmond', 'Washington', 109738839051539, -8, NULL, NULL, 0, 'en_US', 1,
'https://fbcdn-profile-a.akamaihd.net/hprofile-ak-ash1/t5/187654_762118888_7727014_q.jpg', '2014-02-13 04:30:16.400', '2014-02-13 04:30:16.400');

INSERT INTO [WorkHistory] VALUES (762118888,0,20528438720,N'Microsoft',109542932398298,'Software Engineer',	'2005-10-01 00:00:00.000',NULL);
INSERT INTO [WorkHistory] VALUES (762118888,1,549200455135745,N'Havelsan',110265725662722,'Senior Software Engineer','2004-10-01 00:00:00.000','2005-10-01 00:00:00.000');
INSERT INTO [WorkHistory] VALUES (762118888,2,1414185258805261,N'TUBITAK',142231422499061,'Software Engineer, Researcher','2002-10-01 00:00:00.000','2004-10-01 00:00:00.000');
INSERT INTO [WorkHistory] VALUES (762118888,3,104929062881347,N'Aydın Yazılım',109542932398298,'Software Engineer','2001-07-01 00:00:00.000','2002-10-01 00:00:00.000');

INSERT INTO [EducationHistory] VALUES (762118888,0,'Graduate School',105548969479653, N'Ankara University',108654845832522,'Business Administration',196378900380313, 'MBA', 2004);
INSERT INTO [EducationHistory] VALUES (762118888,1,'College',213879365316179, N'Bilkent Üniversitesi',104076956295773,'Computer Science', 0, NULL,2001);
INSERT INTO [EducationHistory] VALUES (762118888,2,'High School',115265641821489, N'Malatya Fen Lisesi, Turkey',0,NULL,0,NULL,1996);


INSERT INTO [ImageMetaFile] VALUES ('AABD4A3C-81A0-482D-8BC9-244F9B8741E2','AABD4A3C-81A0-482D-8BC9-244F9B8741E2','32141998-80F2-4659-83E9-B112C3F649EF', 0, 'image/jpeg', 940369, 1080, 1920, 'http://gingertest.blob.core.windows.net/photos/img_aabd4a3c81a0482d8bc9244f9b8741e2.jpeg', 0, 0, 1, '2014-02-13 04:30:11.013');
INSERT INTO [ImageMetaFile] VALUES ('61ED8D6F-1774-4DEE-840B-712A5DEEC0D7','AABD4A3C-81A0-482D-8BC9-244F9B8741E2','32141998-80F2-4659-83E9-B112C3F649EF', 0, 'image/jpeg', 5976, 200, 200, 'http://gingertest.blob.core.windows.net/photos/img_61ed8d6f17744dee840b712a5deec0d7.jpeg', 0, 1, 1, '2014-02-13 04:30:11.017');
INSERT INTO [ImageMetaFile] VALUES ('44DB6BF3-2ED0-4E21-83E9-CA651B0F1E99','AABD4A3C-81A0-482D-8BC9-244F9B8741E2','32141998-80F2-4659-83E9-B112C3F649EF', 0, 'image/jpeg', 4790, 200, 200, 'http://gingertest.blob.core.windows.net/photos/img_44db6bf32ed04e2183e9ca651b0f1e99.jpeg', 1, 1, 1, '2014-02-13 04:30:11.020');
INSERT INTO [ImageMetaFile] VALUES ('D73E55A0-D426-4E18-865C-EC89ECE9647C','AABD4A3C-81A0-482D-8BC9-244F9B8741E2','32141998-80F2-4659-83E9-B112C3F649EF', 0, 'image/jpeg', 60489, 1080, 1920, 'http://gingertest.blob.core.windows.net/photos/img_d73e55a0d4264e18865cec89ece9647c.jpeg', 1, 0, 1, '2014-02-13 04:30:11.013');
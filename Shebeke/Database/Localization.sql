-- select * from ReactionType
UPDATE [ReactionType] SET [Text] = N'Muhteşem!' WHERE [Id] = 1
UPDATE [ReactionType] SET [Text] = N'Çok afilli!' WHERE [Id] = 2
UPDATE [ReactionType] SET [Text] = N'Budur!' WHERE [Id] = 4
UPDATE [ReactionType] SET [Text] = N'Gözümsün!' WHERE [Id] = 8
UPDATE [ReactionType] SET [Text] = N'O iş öyle değil!' WHERE [Id] = 16
UPDATE [ReactionType] SET [Text] = N'Müthiş komik!' WHERE [Id] = 32
UPDATE [ReactionType] SET [Text] = N'Hiç komik değil!' WHERE [Id] = 64
UPDATE [ReactionType] SET [Text] = N'Bilemiyorum!' WHERE [Id] = 128
UPDATE [ReactionType] SET [Text] = N'Yav he he!' WHERE [Id] = 256
UPDATE [ReactionType] SET [IsEnabled] = 0 WHERE [Id] > 256

-- select * from SplitProperty

UPDATE [SplitProperty] SET [Value] = N'Sesini duyur! <br/>Shebeke seni dünyaya bağlar.' WHERE [Id] = 17;
UPDATE [SplitProperty] SET [Value] = N'Beta''ya hemen katıl!' WHERE [Id] = 18;
UPDATE [SplitProperty] SET [Value] = N'Facebook ile Bağlan' WHERE [Id] = 19;
UPDATE [SplitProperty] SET [Value] = N'Teşekkürler! Sizi bekleme listemize ekledik. Daha fazla beta kullanıcısına hizmet etmeye hazır olur olmaz, sizi bilgilendireceğiz. Lütfen, arkadaşlarınızı bu yeni platforma davet ederek, daha çok insana ulaşmamıza yardımcı olun! Daha fazla <a href=''javascript:void(0);'' class=''inviteLink''>davet</a> göndererek, bekleme sırasındaki yerinizi <span class=''emphasize''>yukarılara taşıyabilirsiniz</span>!' WHERE [Id] = 20;

UPDATE [SplitProperty] SET [Value] = N'Sana reddedemeyeceğin bir teklif yapacağım.' WHERE [Id] = 37;
UPDATE [SplitProperty] SET [Value] = N'Aileye katıl!' WHERE [Id] = 38;
UPDATE [SplitProperty] SET [Value] = N'Yüzünü görelim' WHERE [Id] = 39;
UPDATE [SplitProperty] SET [Value] = N'Sadakatini kanıtlamak için <span class=''emphasize''>daha fazla</span> arkadaşını <a href=''javascript:void(0);'' class=''inviteLink''>davet</a> etmelisin!' WHERE [Id] = 40;

UPDATE [SplitProperty] SET [Value] = N'Þu an dünyada neler mi oluyor? Arkadaşlarımızla, gündemin en sıcak haberlerini burada tartışıyoruz!' WHERE [Id] = 57;
UPDATE [SplitProperty] SET [Value] = N'Beta''ya hemen katıl!' WHERE [Id] = 58;
UPDATE [SplitProperty] SET [Value] = N'Facebook ile Bağlan' WHERE [Id] = 59;
UPDATE [SplitProperty] SET [Value] = N'Teşekkürler! Sizi bekleme listemize ekledik. Daha fazla beta kullanıcısına hizmet etmeye hazır olur olmaz, sizi bilgilendireceğiz. Lütfen, arkadaşlarınızı bu yeni platforma davet ederek, daha çok insana ulaşmamıza yardımcı olun! Daha fazla <a href=''javascript:void(0);'' class=''inviteLink''>davet</a> göndererek, bekleme sırasındaki yerinizi <span class=''emphasize''>yukarılara taşıyabilirsiniz</span>!' WHERE [Id] = 60;

UPDATE [SplitProperty] SET [Value] = N'13 yaş sınırlaması var!<br/>Bu içerik çocuklar için uygun olmayabilir. Yaşınızı teyid için lütfen giriş yapın.' WHERE [Id] = 65;
UPDATE [SplitProperty] SET [Value] = N'13 yaş sınırı var! Yaşını teyid için giriş yap.' WHERE [Id] = 66;

UPDATE [SplitProperty] SET [Value] = N'Paylaşımda bulunan kişinin, sizin arkadaşınız olduğunu teyid etmemiz gerekiyor. Paylaşımı görebilmek için lütfen giriş yapın.' WHERE [Id] = 71;
UPDATE [SplitProperty] SET [Value] = N'Paylaşımı görebilmek için lütfen giriş yapın.' WHERE [Id] = 72;

UPDATE [SplitProperty] SET [Value] = N'Arkadaşlarının ne paylaştığını merak ediyor musun?' WHERE [Id] = 73;
UPDATE [SplitProperty] SET [Value] = N'Arkadaşların bu konuda ne mi dedi?' WHERE [Id] = 74;
UPDATE [SplitProperty] SET [Value] = N'Arkadaşların ne mi paylaştı?' WHERE [Id] = 75;



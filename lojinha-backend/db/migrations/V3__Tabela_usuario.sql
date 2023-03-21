
CREATE TABLE users(

    [Id] INT IDENTITY(1, 1) NOT NULL,
    [user_name] VARCHAR(30) NOT NULL unique default '0',
    [password]  VARCHAR(150) NOT NULL default '0',
    [full_name] VARCHAR(120) NOT NULL,
    [refresh_token] VARCHAR(500) NOT NULL default '0',
    [refresh_token_expiry_time] DATETIME NULL default NULL,
    PRIMARY KEY(Id),
);

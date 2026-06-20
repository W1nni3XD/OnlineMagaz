DO
$$
BEGIN
    UPDATE "Users"
    SET
        "PasswordHash" = '$2a$11$lg8BUXmuwwPAGd3IURhyeuyt2UiLWYGLNJXgDFl7FfbPBiuk7mWt6',
        "Role" = 'Admin',
        "DisplayName" = 'Administrator'
    WHERE "Email" = 'admin@online.magaz';

    IF NOT FOUND THEN
        INSERT INTO "Users" ("Email", "PasswordHash", "Role", "DisplayName", "CreatedAt")
        VALUES (
            'admin@online.magaz',
            '$2a$11$lg8BUXmuwwPAGd3IURhyeuyt2UiLWYGLNJXgDFl7FfbPBiuk7mWt6',
            'Admin',
            'Administrator',
            NOW()
        );
    END IF;
END
$$;
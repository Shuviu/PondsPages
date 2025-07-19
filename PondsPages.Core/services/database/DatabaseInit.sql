CREATE TABLE if not exists author (
                        id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        name TEXT NOT NULL
);

CREATE TABLE if not exists publisher (
                           id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                           name TEXT NOT NULL
);

CREATE TABLE if not exists book (
                      isbn TEXT PRIMARY KEY NOT NULL,
                      title TEXT NOT NULL,
                      publishdate TEXT, 
                      description TEXT,
                      cover_small BLOB,
                      cover_medium BLOB,
                      cover_large BLOB
);

CREATE TABLE if not exists book_author (
                             id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                             book_isbn TEXT NOT NULL,
                             author_id INTEGER NOT NULL,
                             FOREIGN KEY (book_isbn) REFERENCES book(isbn),
                             FOREIGN KEY (author_id) REFERENCES author(id)
);

CREATE TABLE if not exists book_publisher (
                                id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                book_isbn TEXT NOT NULL,
                                publisher_id INTEGER NOT NULL,
                                FOREIGN KEY (book_isbn) REFERENCES book(isbn),
                                FOREIGN KEY (publisher_id) REFERENCES publisher(id)
);
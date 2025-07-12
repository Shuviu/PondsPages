PRAGMA foreign_keys = ON;

-- Drop tables in reverse dependency order to avoid foreign key conflicts
DROP TABLE IF EXISTS book_author;
DROP TABLE IF EXISTS book_publisher;
DROP TABLE IF EXISTS book;
DROP TABLE IF EXISTS author;
DROP TABLE IF EXISTS publisher;

-- Create Tables
CREATE TABLE author (
                        id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        name TEXT NOT NULL
);

CREATE TABLE publisher (
                           id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                           name TEXT NOT NULL
);

CREATE TABLE book (
                      isbn TEXT PRIMARY KEY NOT NULL,
                      title TEXT NOT NULL,
                      publishdate TEXT, -- Storing date as ISO 8601 string 'YYYY-MM-DD'
                      description TEXT,
                      cover_small BLOB,
                      cover_medium BLOB,
                      cover_large BLOB
);

CREATE TABLE book_author (
                             id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                             book_isbn TEXT NOT NULL,
                             author_id INTEGER NOT NULL,
                             FOREIGN KEY (book_isbn) REFERENCES book(isbn),
                             FOREIGN KEY (author_id) REFERENCES author(id)
);

CREATE TABLE book_publisher (
                                id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                book_isbn TEXT NOT NULL,
                                publisher_id INTEGER NOT NULL,
                                FOREIGN KEY (book_isbn) REFERENCES book(isbn),
                                FOREIGN KEY (publisher_id) REFERENCES publisher(id)
);

-- Insert Test Data

-- Authors
INSERT INTO author (name) VALUES
                              ('J.K. Rowling'),
                              ('Stephen King'),
                              ('Agatha Christie'),
                              ('George Orwell');

-- Publishers
INSERT INTO publisher (name) VALUES
                                 ('Bloomsbury Publishing'),
                                 ('Scribner'),
                                 ('HarperCollins'),
                                 ('Penguin Books');

-- Books (Note: BLOB fields are left NULL for simplicity in test data)
INSERT INTO book (isbn, title, publishdate, description, cover_small, cover_medium, cover_large)VALUES
                                                                                         ('9780747532743', 'Harry Potter and the Philosopher''s Stone', '1997-06-26', 'The first novel in the Harry Potter series.', X'01', X'02', X'03'),
                                                                                         ('9780451169518', 'It', '1986-09-15', 'A horror novel about an evil entity.', X'04', X'05', X'06'),
                                                                                         ('9780062073488', 'And Then There Were None', '1939-11-06', 'A classic mystery novel by Agatha Christie.', X'07', X'08', X'09'),
                                                                                         ('9780451524935', '1984', '1949-06-08', 'A dystopian social science fiction novel.', X'0A', X'0B', X'0C');


-- Book Authors (linking books to authors)
-- Harry Potter by J.K. Rowling
INSERT INTO book_author (book_isbn, author_id) VALUES
    ('9780747532743', (SELECT id FROM author WHERE name = 'J.K. Rowling'));

-- It by Stephen King
INSERT INTO book_author (book_isbn, author_id) VALUES
    ('9780451169518', (SELECT id FROM author WHERE name = 'Stephen King'));

-- And Then There Were None by Agatha Christie
INSERT INTO book_author (book_isbn, author_id) VALUES
    ('9780062073488', (SELECT id FROM author WHERE name = 'Agatha Christie'));

-- 1984 by George Orwell
INSERT INTO book_author (book_isbn, author_id) VALUES
    ('9780451524935', (SELECT id FROM author WHERE name = 'George Orwell'));


-- Book Publishers (linking books to publishers)
-- Harry Potter by Bloomsbury Publishing
INSERT INTO book_publisher (book_isbn, publisher_id) VALUES
    ('9780747532743', (SELECT id FROM publisher WHERE name = 'Bloomsbury Publishing'));

-- It by Scribner
INSERT INTO book_publisher (book_isbn, publisher_id) VALUES
    ('9780451169518', (SELECT id FROM publisher WHERE name = 'Scribner'));

-- And Then There Were None by HarperCollins
INSERT INTO book_publisher (book_isbn, publisher_id) VALUES
    ('9780062073488', (SELECT id FROM publisher WHERE name = 'HarperCollins'));

-- 1984 by Penguin Books
INSERT INTO book_publisher (book_isbn, publisher_id) VALUES
    ('9780451524935', (SELECT id FROM publisher WHERE name = 'Penguin Books'));
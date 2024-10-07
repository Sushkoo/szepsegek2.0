CREATE TABLE dolgozok (
  dolgozoId INT PRIMARY KEY,
  nev VARCHAR(255),
  fizetes INT,
  telefonszam INT,
  foglalasId INT,
  FOREIGN KEY (foglalasId) REFERENCES foglalasok(foglalasId)
);

CREATE TABLE ugyfel (
  ugyfelId INT PRIMARY KEY,
  nev VARCHAR(255),
  szolgaltatasId INT,
  FOREIGN KEY (szolgaltatasId) REFERENCES szolgaltatasok(szolgaltatasId)
);

CREATE TABLE foglalasok (
  foglalasId INT PRIMARY KEY,
  idopont DATETIME,
  ugyfelId INT,
  FOREIGN KEY (ugyfelId) REFERENCES ugyfel(ugyfelId)
);

CREATE TABLE szolgaltatasok (
  szolgaltatasId INT PRIMARY KEY,
  szolgaltatas VARCHAR(255),
  dolgozoId INT,
  FOREIGN KEY (dolgozoId) REFERENCES dolgozok(dolgozoId)
);
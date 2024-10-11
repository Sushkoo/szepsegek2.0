-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2024. Okt 11. 13:51
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `szepsegek2`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `dolgozok`
--

CREATE TABLE `dolgozok` (
  `DolgozoID` int(11) NOT NULL,
  `DolgozoVezetekNev` varchar(255) NOT NULL,
  `DolgozoKeresztNev` varchar(255) NOT NULL,
  `DolgozoTelefon` varchar(255) NOT NULL,
  `DolgozoEmail` varchar(255) NOT NULL,
  `DolgozoStatusz` tinyint(1) NOT NULL,
  `SzolgaltatasID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `dolgozok`
--

INSERT INTO `dolgozok` (`DolgozoID`, `DolgozoVezetekNev`, `DolgozoKeresztNev`, `DolgozoTelefon`, `DolgozoEmail`, `DolgozoStatusz`, `SzolgaltatasID`) VALUES
(1, 'Gáspár', 'Laci', '+36204206969', 'gasparlaci@gmail.com', 1, 1),
(2, 'Gáspár', 'Győző', '+36204209696', 'gaspargyozo@gmail.com', 1, 2),
(3, 'Kata', 'Pacal', '+36205554444', 'sokateszek@gmail.com', 1, 7),
(4, 'Bagi', 'Méla', '+36504206969', 'pekka@gmail.com', 1, 5),
(5, 'Lakatos', 'Lajos Herceg Danyi Hugo', '+36203214321', 'cigany@gmail.com', 1, 6);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `foglalasok`
--

CREATE TABLE `foglalasok` (
  `FoglalasID` int(11) NOT NULL,
  `SzolgaltatasID` int(11) NOT NULL,
  `DolgozoID` int(11) NOT NULL,
  `Ido` varchar(255) NOT NULL,
  `OraPerc` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `foglalasok`
--

INSERT INTO `foglalasok` (`FoglalasID`, `SzolgaltatasID`, `DolgozoID`, `Ido`, `OraPerc`) VALUES
(1, 1, 1, '2024-10-12', '13:30'),
(2, 2, 2, '2024-10-12', '13:30'),
(3, 1, 1, '2024-10-12', '14:30'),
(4, 1, 1, '2024-10-12', '8:0'),
(5, 1, 1, '2024-10-12', '15:49');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `szolgaltatasok`
--

CREATE TABLE `szolgaltatasok` (
  `SzolgaltatasID` int(11) NOT NULL,
  `SzolgaltatasKategoria` varchar(255) NOT NULL,
  `SzolgaltatasIdotartam` int(11) NOT NULL,
  `SzolgaltatasAr` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `szolgaltatasok`
--

INSERT INTO `szolgaltatasok` (`SzolgaltatasID`, `SzolgaltatasKategoria`, `SzolgaltatasIdotartam`, `SzolgaltatasAr`) VALUES
(1, 'Körömcibálás', 30, 2000),
(2, 'Körömflexelés', 120, 4000),
(3, 'Hajhuzigálás', 15, 3000),
(4, 'Szemöldöktépegetés', 55, 5000),
(5, 'Lábujjsimogatás', 20, 1000),
(6, 'Pofon', 1, 10),
(7, 'Dögönyözés', 60, 6000);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `dolgozok`
--
ALTER TABLE `dolgozok`
  ADD PRIMARY KEY (`DolgozoID`),
  ADD KEY `test` (`SzolgaltatasID`);

--
-- A tábla indexei `foglalasok`
--
ALTER TABLE `foglalasok`
  ADD PRIMARY KEY (`FoglalasID`),
  ADD KEY `szolgaltatasid_innerjoin` (`SzolgaltatasID`),
  ADD KEY `dolgozoid_innerjoin` (`DolgozoID`);

--
-- A tábla indexei `szolgaltatasok`
--
ALTER TABLE `szolgaltatasok`
  ADD PRIMARY KEY (`SzolgaltatasID`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `dolgozok`
--
ALTER TABLE `dolgozok`
  MODIFY `DolgozoID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `foglalasok`
--
ALTER TABLE `foglalasok`
  MODIFY `FoglalasID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `szolgaltatasok`
--
ALTER TABLE `szolgaltatasok`
  MODIFY `SzolgaltatasID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `dolgozok`
--
ALTER TABLE `dolgozok`
  ADD CONSTRAINT `test` FOREIGN KEY (`SzolgaltatasID`) REFERENCES `szolgaltatasok` (`SzolgaltatasID`);

--
-- Megkötések a táblához `foglalasok`
--
ALTER TABLE `foglalasok`
  ADD CONSTRAINT `dolgozoid_innerjoin` FOREIGN KEY (`DolgozoID`) REFERENCES `dolgozok` (`DolgozoID`),
  ADD CONSTRAINT `szolgaltatasid_innerjoin` FOREIGN KEY (`SzolgaltatasID`) REFERENCES `szolgaltatasok` (`SzolgaltatasID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

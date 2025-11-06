CREATE DATABASE  IF NOT EXISTS `OpusOBDtest` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `OpusOBDtest`;

DROP TABLE IF EXISTS `InspectionData`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `InspectionData` (
  `IdInspectionData` int NOT NULL,
  `IdStation` varchar(20) DEFAULT NULL,
  `FECHA` datetime DEFAULT NULL,
  `DeviceID` varchar(20) DEFAULT NULL,
  `VINhx` varchar(100) DEFAULT NULL,
  `MILhx` varchar(100) DEFAULT NULL,
  `DTChx` varchar(100) DEFAULT NULL,
  `VIN` varchar(100) DEFAULT NULL,
  `MIL` boolean DEFAULT NULL,
  `MSI` varchar(3) DEFAULT NULL,
  `CCM` varchar(3) DEFAULT NULL,
  `CMB` varchar(3) DEFAULT NULL,
  `O2S` varchar(3) DEFAULT NULL,
  `CAT` varchar(3) DEFAULT NULL,
  `CCC` varchar(3) DEFAULT NULL,
  `EVS` varchar(3) DEFAULT NULL,
  `SAS` varchar(3) DEFAULT NULL,
  `FAA` varchar(3) DEFAULT NULL,
  `O2C` varchar(3) DEFAULT NULL,
  `DTC` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`IdInspectionData`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;



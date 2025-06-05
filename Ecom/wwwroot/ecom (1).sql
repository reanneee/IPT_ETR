-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 05, 2025 at 03:59 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `ecom`
--

-- --------------------------------------------------------

--
-- Table structure for table `cart`
--

CREATE TABLE `cart` (
  `CartID` int(11) NOT NULL,
  `UserID` int(11) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `cartitems`
--

CREATE TABLE `cartitems` (
  `CartItemID` int(11) NOT NULL,
  `CartID` int(11) DEFAULT NULL,
  `ProductID` int(11) DEFAULT NULL,
  `Quantity` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `categories`
--

CREATE TABLE `categories` (
  `CategoryID` int(11) NOT NULL,
  `CategoryName` varchar(100) NOT NULL,
  `Description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `categories`
--

INSERT INTO `categories` (`CategoryID`, `CategoryName`, `Description`) VALUES
(1, 'Aglaonema', 'A popular indoor plant known for its vibrant foliage'),
(2, 'Air Purifying Plants', 'Plants that help filter and clean indoor air'),
(3, 'Anthurium', 'A tropical plant valued for its glossy leaves and striking flowers'),
(4, 'Beginner-friendly', 'Easy-to-care-for plants suitable for new plant owners');

-- --------------------------------------------------------

--
-- Table structure for table `guestcart`
--

CREATE TABLE `guestcart` (
  `SessionID` varchar(100) NOT NULL,
  `ProductID` int(11) NOT NULL,
  `Quantity` int(11) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `orderitems`
--

CREATE TABLE `orderitems` (
  `OrderItemID` int(11) NOT NULL,
  `OrderID` int(11) DEFAULT NULL,
  `ProductID` int(11) DEFAULT NULL,
  `Quantity` int(11) DEFAULT NULL,
  `PriceAtPurchase` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `orders`
--

CREATE TABLE `orders` (
  `OrderID` int(11) NOT NULL,
  `UserID` int(11) DEFAULT NULL,
  `OrderDate` datetime DEFAULT current_timestamp(),
  `Status` enum('Pending','Processing','Shipped','Delivered','Cancelled') DEFAULT 'Pending',
  `TotalAmount` decimal(10,2) DEFAULT NULL,
  `ShippingAddress` text DEFAULT NULL,
  `TrackingNumber` varchar(100) DEFAULT NULL,
  `ShippedDate` datetime DEFAULT NULL,
  `DeliveredDate` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `products`
--

CREATE TABLE `products` (
  `ProductID` int(11) NOT NULL,
  `CategoryID` int(11) DEFAULT NULL,
  `ProductName` varchar(100) NOT NULL,
  `Description` text DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL,
  `Stock` int(11) NOT NULL,
  `Image` varchar(255) DEFAULT NULL,
  `Visible` tinyint(1) DEFAULT 1,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `products`
--

INSERT INTO `products` (`ProductID`, `CategoryID`, `ProductName`, `Description`, `Price`, `Stock`, `Image`, `Visible`, `CreatedAt`) VALUES
(1, 1, 'Aglaonema Super Red', 'The Aglaonema is an adorable plant with leaves resembling vegetables, originating from the Aglaonema family. It is perfect as a tabletop plant.', 1100.00, 10, '/uploads/products/aglaonema_super_red.jpg', 1, '2025-06-05 09:53:52'),
(2, 1, 'Aglaonema Super White', 'The Aglaonema is an adorable plant with leaves resembling vegetables, originating from the Aglaonema family. It is perfect as a tabletop plant.', 1100.00, 10, '/uploads/products/aglaonema_super_white.jpg', 1, '2025-06-05 09:53:52'),
(3, 1, 'Aglaonema White Edge', 'The Aglaonema is an adorable plant with leaves resembling vegetables, originating from the Aglaonema family', 1500.00, 10, '/uploads/products/aglaonema_white_edge.jpg', 1, '2025-06-05 09:53:52'),
(4, 1, 'Aglaonema Hongmai', 'The Aglaonema is an adorable plant with leaves resembling vegetables, originating from the Aglaonema family. It is perfect as a tabletop plant.', 1100.00, 10, '/uploads/products/aglaonema_hongmai.jpg', 1, '2025-06-05 09:53:52'),
(5, 2, 'Calathea Orbifolia', 'Calathea Orbifolia is a stunning tropical houseplant prized for its large, round leaves adorned with silvery-green stripes. Native to Bolivia, this plant is part of the Marantaceae (prayer plant) family and is well-known for its elegant foliage that moves slightly in response to light and dark—an effect known as nyctinasty. Its bold, decorative appearance makes it a favorite among interior plant lovers.', 990.00, 10, '/uploads/products/Calathea_Orbifolia.jpg', 1, '2025-06-05 09:57:41'),
(6, 2, 'Zz Raven', 'The Raven ZZ plant is one of the new and rare varieties of plants. It features bright green new growth which matures to a rich purple-black dark foliage. This variety adds a beautiful contrast to your other house plants because of its beautiful pigmentation. It can also thrive in both natural and artificial lighting for it to grow its beautiful leaves.', 1900.00, 10, '/uploads/products/Zz_Raven.jpg', 1, '2025-06-05 09:57:41'),
(7, 2, 'Everfresh Tree', 'The Ever Fresh Tree, a native of Japan, is renowned for its lush foliage and graceful silhouette. Its vibrant green leaves, meticulously shaped, create a serene ambiance in gardens and landscapes. With its enduring beauty and tranquil presence, the Ever Fresh Tree stands as a symbol of natural harmony and tranquility.', 800.00, 10, '/uploads/products/Everfresh_Tree.jpg', 1, '2025-06-05 09:57:41'),
(8, 2, 'Ponytail Palm', 'The Ponytail Palm is a unique and eye-catching plant that looks like a miniature tree with a thick, bulbous trunk and long, curly, strap-like leaves that cascade gracefully, resembling a ponytail. Despite its name, it is not a true palm but rather a member of the agave family.', 3500.00, 10, '/uploads/products/Ponytail_Palm.jpg', 1, '2025-06-05 09:57:41'),
(9, 3, 'Anthurium Clarinervium', 'Anthurium clarinervium, also known as the Velvet Cardboard Anthurium, is a stunning plant cherished for its unique foliage. It belongs to the Araceae family and is native to the rainforests of southern Mexico.', 450.00, 10, '/uploads/products/Anthurium_Clarinervium.jpg', 1, '2025-06-05 09:58:30'),
(10, 3, 'Anthurium Red', 'Anthurium Red, often known as the Flamingo Flower or Laceleaf, is a popular ornamental plant celebrated for its glossy, heart-shaped red spathes and contrasting yellow spadix. It features dark green, leathery leaves and blooms nearly year-round under ideal conditions. This plant is a favorite for indoor decoration due to its bold, tropical appearance and long-lasting flowers.', 600.00, 10, '/uploads/products/Anthurium_Red.jpg', 1, '2025-06-05 09:58:30'),
(11, 3, 'Anthurium Pink', 'Anthurium Pink is a charming variety of the Anthurium genus, known for its soft pink spathes that symbolize grace and elegance. It shares similar care requirements with Anthurium Red and is frequently used in floral arrangements and as an indoor plant for adding a pop of gentle color.', 600.00, 10, '/uploads/products/Anthurium_Pink.jpg', 1, '2025-06-05 09:58:30'),
(12, 3, 'Anthurium Clarinervium', 'Anthurium Clarinervium is a stunning foliage plant admired for its velvety, dark green leaves with striking white veins. Native to Mexico, this variety does not produce prominent flowers like other anthuriums, but its bold foliage makes it a standout in any plant collection.', 1750.00, 10, '/uploads/products/Anthurium_Clarinervium_2.jpg', 1, '2025-06-05 09:58:30'),
(13, 3, 'Anthurium Crystallinum', 'Anthurium Crystallinum is another jewel-like foliage plant, closely related to Clarinervium, with velvety leaves and prominent white veining. Its heart-shaped leaves are typically larger and lighter green, giving it a lush, tropical aesthetic.', 1990.00, 10, '/uploads/products/Anthurium_Crystallinum.jpg', 1, '2025-06-05 09:58:30'),
(14, 4, 'Sansevieria Bacularis', 'The Sansevieria Bacularis is the best plant for novice plant owners because it is almost indestructible. This can also serve as a good aesthetic addition to a modern interior design setting. As an additional benefit, this plant naturally purifies the air and improves humidity, hence ensuring a healthy atmosphere indoors.', 200.00, 10, '/uploads/products/Sansevieria_Bacularis.jpg', 1, '2025-06-05 09:59:00'),
(15, 4, 'Fortune Plant', 'The Fortune Plant was aptly named because it creates a cheerful appearance to the atmosphere. It is easy to take care of and can survive best indoors. Placing a fortune plant in every room in your house can help create a bright and cheerful ambiance, regardless of whether it would bring you good luck.', 950.00, 10, '/uploads/products/Fortune_Plant.jpg', 1, '2025-06-05 09:59:00'),
(16, 4, 'Heartleaf Philodendron', 'The Philodendron Heartleaf is a vining plant that has fast-growing heart-shaped leaves that are dark and glossy green that are almost transparent. This trailing plant can be placed in bookshelves and plant hangers where its trailing leaves can beautifully spill out. This is a great plant for novice plant owners because it requires minimal maintenance.', 220.00, 10, '/uploads/products/Heartleaf_Philodendron.jpg', 1, '2025-06-05 09:59:00');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `UserID` int(11) NOT NULL,
  `Firstname` varchar(50) NOT NULL,
  `Middlename` varchar(50) DEFAULT NULL,
  `Lastname` varchar(50) NOT NULL,
  `Address` text NOT NULL,
  `Role` enum('Admin','Customer','Guest') DEFAULT 'Customer',
  `Email` varchar(100) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `cart`
--
ALTER TABLE `cart`
  ADD PRIMARY KEY (`CartID`),
  ADD KEY `UserID` (`UserID`);

--
-- Indexes for table `cartitems`
--
ALTER TABLE `cartitems`
  ADD PRIMARY KEY (`CartItemID`),
  ADD UNIQUE KEY `unique_cart_product` (`CartID`,`ProductID`),
  ADD KEY `ProductID` (`ProductID`);

--
-- Indexes for table `categories`
--
ALTER TABLE `categories`
  ADD PRIMARY KEY (`CategoryID`);

--
-- Indexes for table `guestcart`
--
ALTER TABLE `guestcart`
  ADD PRIMARY KEY (`SessionID`,`ProductID`),
  ADD KEY `ProductID` (`ProductID`);

--
-- Indexes for table `orderitems`
--
ALTER TABLE `orderitems`
  ADD PRIMARY KEY (`OrderItemID`),
  ADD KEY `OrderID` (`OrderID`),
  ADD KEY `ProductID` (`ProductID`);

--
-- Indexes for table `orders`
--
ALTER TABLE `orders`
  ADD PRIMARY KEY (`OrderID`),
  ADD KEY `UserID` (`UserID`);

--
-- Indexes for table `products`
--
ALTER TABLE `products`
  ADD PRIMARY KEY (`ProductID`),
  ADD KEY `CategoryID` (`CategoryID`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`UserID`),
  ADD UNIQUE KEY `Email` (`Email`),
  ADD KEY `idx_email` (`Email`),
  ADD KEY `idx_role` (`Role`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `cart`
--
ALTER TABLE `cart`
  MODIFY `CartID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `cartitems`
--
ALTER TABLE `cartitems`
  MODIFY `CartItemID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `categories`
--
ALTER TABLE `categories`
  MODIFY `CategoryID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `orderitems`
--
ALTER TABLE `orderitems`
  MODIFY `OrderItemID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `orders`
--
ALTER TABLE `orders`
  MODIFY `OrderID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `products`
--
ALTER TABLE `products`
  MODIFY `ProductID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `cart`
--
ALTER TABLE `cart`
  ADD CONSTRAINT `cart_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Constraints for table `cartitems`
--
ALTER TABLE `cartitems`
  ADD CONSTRAINT `cartitems_ibfk_1` FOREIGN KEY (`CartID`) REFERENCES `cart` (`CartID`) ON DELETE CASCADE,
  ADD CONSTRAINT `cartitems_ibfk_2` FOREIGN KEY (`ProductID`) REFERENCES `products` (`ProductID`);

--
-- Constraints for table `guestcart`
--
ALTER TABLE `guestcart`
  ADD CONSTRAINT `guestcart_ibfk_1` FOREIGN KEY (`ProductID`) REFERENCES `products` (`ProductID`);

--
-- Constraints for table `orderitems`
--
ALTER TABLE `orderitems`
  ADD CONSTRAINT `orderitems_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `orders` (`OrderID`) ON DELETE CASCADE,
  ADD CONSTRAINT `orderitems_ibfk_2` FOREIGN KEY (`ProductID`) REFERENCES `products` (`ProductID`);

--
-- Constraints for table `orders`
--
ALTER TABLE `orders`
  ADD CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`);

--
-- Constraints for table `products`
--
ALTER TABLE `products`
  ADD CONSTRAINT `products_ibfk_1` FOREIGN KEY (`CategoryID`) REFERENCES `categories` (`CategoryID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

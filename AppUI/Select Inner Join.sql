SELECT 
Title,
Amount, 
TransactionInfo.Company,
TransactionInfo.Invoice,
TransactionType.Type,
Account.Balance,
User.FirstName
FROM TransactionFull
INNER JOIN TransactionInfo ON TransactionInfo.Id = TransactionFull.Id
INNER JOIN TransactionType ON TransactionType.Id = TransactionFull.Id
INNER JOIN Account ON Account.Id = TransactionFull.Id
INNER JOIN User ON User.Id = TransactionFull.Id;
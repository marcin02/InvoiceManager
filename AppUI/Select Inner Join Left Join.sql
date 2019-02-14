SELECT TransactionFullId, Title, Amount, Balance, CreationDate, Users.FirstName, TransactionType.Type, TransactionInfo.Company, TransactionInfo.Invoice
FROM TransactionFull
INNER JOIN Users ON Users.UserId = TransactionFull.TransactionFullId
INNER JOIN TransactionType ON TransactionType.TransactionTypeId = TransactionFull.TransactionFullId
LEFT JOIN TransactionInfo ON TransactionInfo.TransactionFull_FK = TransactionFull.TransactionFullId;
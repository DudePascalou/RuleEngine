RuleEngine
==========

A simple rule engine.

It can resolve conditions consisting of the comparison of two operands.
Conditions can be associated with priorities.

Comparers supported are :
 - Equal
 - Not Equal
 - Greater
 - Lesser
 - GreaterOrEqual
 - LesserOrEqual

Associations supported are :
 - And
 - Or

The input is a list of condition entities.
The list is parsed into a condition tree.
The output is a boolean.

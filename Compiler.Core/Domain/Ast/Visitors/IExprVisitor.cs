﻿﻿﻿using Compiler.Domain.Ast.Expressions;

namespace Compiler.Domain.Ast.Visitors;

public interface IExprVisitor<T>
{
    T VisitNumber(NumberExpr expr);
    T VisitFloat(FloatExpr expr);
    T VisitBoolean(BooleanExpr expr);
    T VisitBinary(BinaryExpr expr);
    T VisitGroup(GroupExpr expr);
    T VisitDevice(DeviceExpr expr);
    T VisitLoadDevice(LoadDeviceExpr expr);
    T VisitIdentifier(IdentifierExpr expr);
    T VisitCall(CallExpr expr);
    T VisitDeviceProperty(DevicePropertyExpr expr);
    T VisitAssignment(AssignmentExpr expr);
    T VisitMemberAccess(MemberAccessExpr expr);
    T VisitMemberAssignment(MemberAssignmentExpr expr);
    T VisitStaticMethodCall(StaticMethodCallExpr expr);
    T VisitMethodCall(MethodCallExpr expr);
}


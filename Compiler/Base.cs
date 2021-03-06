using Library;
using static Library.Lib;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using static Compiler.FeelParser;
using static Compiler.Compiler_static;

namespace Compiler
{
public partial class Namespace{
public Namespace(string name , string imports ){this.name = name;
this.imports = imports;
}
public string name;
public string imports;
}
public partial class Result{
public Result(object data , string text , string permission , bool is_virtual , bool isDefine , bool isMutable , string rootID ){this.data = data;
this.text = text;
this.permission = permission;
this.is_virtual = is_virtual;
this.isDefine = isDefine;
this.isMutable = isMutable;
this.rootID = rootID;
}
public object data;
public string text;
public string permission;
public bool is_virtual;
public bool isDefine;
public bool isMutable;
public string rootID;
}
public partial class Result{
public Result ( object data  = null,  string text  = ""){this.data=data;
this.text=text;
permission="public";
is_virtual=false;
isDefine=false;
isMutable=false;
}
}
public partial class FeelLangVisitorCore:FeelParserBaseVisitor<object>{
public FeelLangVisitorCore(string self_ID , string super_ID , List<string> self_property_content , HashSet<string> all_ID_set , Stack<HashSet<string>> cuttent_ID_set , HashSet<string> type_Id_set ){this.self_ID = self_ID;
this.super_ID = super_ID;
this.self_property_content = self_property_content;
this.all_ID_set = all_ID_set;
this.cuttent_ID_set = cuttent_ID_set;
this.type_Id_set = type_Id_set;
}
public string self_ID;
public string super_ID;
public List<string> self_property_content;
public HashSet<string> all_ID_set;
public Stack<HashSet<string>> cuttent_ID_set;
public HashSet<string> type_Id_set;
public  virtual  bool Has_ID( string id ){
return all_ID_set.Contains(id)||cuttent_ID_set.Peek().Contains(id);
}
public  virtual  void Add_ID( string id ){
cuttent_ID_set.Peek().Add(id);
}
public  virtual  void Add_current_set(){
foreach (var item in cuttent_ID_set.Peek()){
all_ID_set.Add(item);
}
cuttent_ID_set.Push((new HashSet<string>()));
}
public  virtual  void Delete_current_set(){
all_ID_set.ExceptWith(cuttent_ID_set.Peek());
cuttent_ID_set.Pop();
}
public  virtual  bool Is_type( string id ){
return type_Id_set.Contains(id);
}
public  virtual  void Add_type( string id ){
type_Id_set.Add(id);
}
public  virtual  string ProcessFunctionSupport( FunctionSupportStatementContext[] items ){
var obj = "";
foreach (var item in items){
obj+=Visit(item);
}
return obj;
}
}
public partial class FeelLangVisitorCore{
public FeelLangVisitorCore (){self_ID="";
super_ID="";
self_property_content=(new List<string>());
all_ID_set=(new HashSet<string>());
cuttent_ID_set=(new Stack<HashSet<string>>());
type_Id_set=(new HashSet<string>());
cuttent_ID_set.Push((new HashSet<string>()));
}
}
public partial class FeelLangVisitorBase:FeelLangVisitorCore{
public FeelLangVisitorBase(){}
public  override  object VisitProgram( ProgramContext context ){
var StatethisntList = context.statement();
var result = "";
foreach (var item in StatethisntList){
result+=VisitStatement(item);
}
return result;
}
public  override  object VisitId( IdContext context ){
var r = (new Result("var"));
var first = ((Result)Visit(context.GetChild(0)));
r.permission=first.permission;
r.text=first.text;
r.is_virtual=first.is_virtual;
if ( context.ChildCount>=2 ) {
foreach (var i in 1.Up_until(context.ChildCount)){
var other = ((Result)Visit(context.GetChild(i)));
r.text+=(new System.Text.StringBuilder().Append("_").Append(other.text)).To_Str();
}
}
if ( r.text==self_ID ) {
r.text="this";
}
else if ( r.text==super_ID ) {
r.text="base";
}
else if ( keywords.Exists((t)=>t==r.text) ) {
r.text=(new System.Text.StringBuilder().Append("@").Append(r.text)).To_Str();
}
r.rootID=r.text;
return r;
}
public  override  object VisitIdItem( IdItemContext context ){
var r = (new Result("var"));
if ( context.typeAny()!=null ) {
r.text+=context.typeAny().GetText();
r.is_virtual=true;
return r;
}
var id = context.Identifier().GetText();
r.text+=id;
r.is_virtual=true;
if ( id[(0)]=='_' ) {
r.permission="protected internal";
if ( id[(1)].Is_lower() ) {
r.isMutable=true;
}
}
else if ( id[(0)].Is_lower() ) {
r.isMutable=true;
}
return r;
}
public  override  object VisitVarId( VarIdContext context ){
if ( context.Discard()!=null ) {
return "_";
}
else {
var id = ((Result)Visit(context.id())).text;
if ( Has_ID(id) ) {
return id;
}
else {
Add_ID(id);
return id;
}
}
}
public  override  object VisitVarIdType( VarIdTypeContext context ){
if ( context.Discard()!=null ) {
return "_";
}
else {
var id = ((Result)Visit(context.id())).text;
if ( !Has_ID(id) ) {
Add_ID(id);
}
return Visit(context.typeType())+" "+id;
}
}
public  override  object VisitBoolExpr( BoolExprContext context ){
var r = (new Result());
if ( context.t.Type==TrueLiteral ) {
r.data=TargetTypeBool;
r.text=T;
}
else if ( context.t.Type==FalseLiteral ) {
r.data=TargetTypeBool;
r.text=F;
}
return r;
}
public  override  object VisitAnnotationSupport( AnnotationSupportContext context ){
return ((string)Visit(context.annotation()));
}
public  override  object VisitAnnotation( AnnotationContext context ){
var obj = "";
var r = ((string)Visit(context.annotationList()));
if ( r!="" ) {
obj+=r;
}
return obj;
}
public  override  object VisitAnnotationList( AnnotationListContext context ){
var obj = "";
foreach (var (i,v) in context.annotationItem().WithIndex()){
var txt = ((string)Visit(v));
if ( txt!="" ) {
obj+=txt;
}
}
return obj;
}
public  override  object VisitAnnotationItem( AnnotationItemContext context ){
var obj = "";
var id = "";
if ( context.id().Length==2 ) {
id=(new System.Text.StringBuilder().Append(((Result)Visit(context.id(0))).text).Append(":")).To_Str();
obj+=((Result)Visit(context.id(1))).text;
}
else {
obj+=((Result)Visit(context.id(0))).text;
}
switch (obj) {
case "get" :
{ self_property_content.Append("get;");
return "";
} break;
case "set" :
{ self_property_content.Append("set;");
return "";
} break;
}
if ( context.tuple()!=null ) {
obj+=((Result)Visit(context.tuple())).text;
}
if ( id!="" ) {
obj=id+obj;
}
obj="["+obj+"]";
return obj;
}
}
public partial class Compiler_static {
public const string Terminate = ";";
public const string Wrap = "\r\n";
public const string TargetTypeAny = "object";
public const string TargetTypeInt = "int";
public const string TargetTypeNum = "double";
public const string TargetTypeI8 = "sbyte";
public const string TargetTypeI16 = "short";
public const string TargetTypeI32 = "int";
public const string TargetTypeI64 = "long";
public const string TargetTypeU8 = "byte";
public const string TargetTypeU16 = "ushort";
public const string TargetTypeU32 = "uint";
public const string TargetTypeU64 = "ulong";
public const string TargetTypeF32 = "float";
public const string TargetTypeF64 = "double";
public const string TargetTypeBool = "bool";
public const string T = "true";
public const string F = "false";
public const string TargetTypeChr = "char";
public const string TargetTypeStr = "string";
public const string TargetTypeLst = "List";
public const string TargetTypeSet = "Hashset";
public const string TargetTypeDic = "Dictionary";
public const string BlockLeft = "{";
public const string BlockRight = "}";
public const string Task = "System.Threading.Tasks.Task";
public static List<string> keywords = List_of("abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while");
}
}

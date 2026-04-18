using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;

namespace Grondo.Tests
{
    /// <summary>
    /// Property-based tests verifying the three monad laws for Maybe, Result, and Either:
    /// 1. Left identity:  return(a).Bind(f)    == f(a)
    /// 2. Right identity: m.Bind(return)       == m
    /// 3. Associativity:  m.Bind(f).Bind(g)    == m.Bind(x => f(x).Bind(g))
    /// </summary>
    [TestClass]
    public class MonadLawsTests : BaseTest
    {
        // --- Maybe<T> ---

        [TestMethod]
        public void Maybe_LeftIdentity() =>
            Prop.ForAll<int>(a =>
            {
                Func<int, Maybe<int>> f = x => Maybe<int>.Some(x + 1);
                return Maybe<int>.Some(a).Bind(f).Equals(f(a));
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Maybe_RightIdentity_Some() =>
            Prop.ForAll<int>(a =>
            {
                var m = Maybe<int>.Some(a);
                return m.Bind(Maybe<int>.Some).Equals(m);
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Maybe_RightIdentity_None() =>
            Maybe<int>.None.Bind(Maybe<int>.Some).Equals(Maybe<int>.None).Should().BeTrue();

        [TestMethod]
        public void Maybe_Associativity() =>
            Prop.ForAll<int>(a =>
            {
                Func<int, Maybe<int>> f = x => Maybe<int>.Some(x * 2);
                Func<int, Maybe<int>> g = x => Maybe<int>.Some(x + 3);
                var m = Maybe<int>.Some(a);
                return m.Bind(f).Bind(g).Equals(m.Bind(x => f(x).Bind(g)));
            }).QuickCheckThrowOnFailure();

        // --- Result<T> ---

        [TestMethod]
        public void Result_LeftIdentity() =>
            Prop.ForAll<int>(a =>
            {
                Func<int, Result<int>> f = x => Result<int>.Success(x + 1);
                return Result<int>.Success(a).Bind(f).Equals(f(a));
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Result_RightIdentity_Success() =>
            Prop.ForAll<int>(a =>
            {
                var m = Result<int>.Success(a);
                return m.Bind(Result<int>.Success).Equals(m);
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Result_RightIdentity_Failure() =>
            Prop.ForAll<string>(message =>
            {
                string safe = message ?? "error";
                var m = Result<int>.Failure(safe);
                return m.Bind(Result<int>.Success).Equals(m);
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Result_Associativity() =>
            Prop.ForAll<int>(a =>
            {
                Func<int, Result<int>> f = x => Result<int>.Success(x * 2);
                Func<int, Result<int>> g = x => Result<int>.Success(x + 3);
                var m = Result<int>.Success(a);
                return m.Bind(f).Bind(g).Equals(m.Bind(x => f(x).Bind(g)));
            }).QuickCheckThrowOnFailure();

        // --- Either<L, R> ---

        [TestMethod]
        public void Either_LeftIdentity() =>
            Prop.ForAll<int>(a =>
            {
                Func<int, Either<string, int>> f = x => Either<string, int>.FromRight(x + 1);
                return Either<string, int>.FromRight(a).Bind(f).Equals(f(a));
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Either_RightIdentity() =>
            Prop.ForAll<int>(a =>
            {
                var m = Either<string, int>.FromRight(a);
                return m.Bind(Either<string, int>.FromRight).Equals(m);
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Either_Associativity() =>
            Prop.ForAll<int>(a =>
            {
                Func<int, Either<string, int>> f = x => Either<string, int>.FromRight(x * 2);
                Func<int, Either<string, int>> g = x => Either<string, int>.FromRight(x + 3);
                var m = Either<string, int>.FromRight(a);
                return m.Bind(f).Bind(g).Equals(m.Bind(x => f(x).Bind(g)));
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Either_Left_ShortCircuitsBind() =>
            Prop.ForAll<string>(leftValue =>
            {
                string safe = leftValue ?? "left";
                var m = Either<string, int>.FromLeft(safe);
                Func<int, Either<string, int>> f = x => Either<string, int>.FromRight(x + 1);
                return m.Bind(f).Equals(m);
            }).QuickCheckThrowOnFailure();

        /// <summary>
        /// Functor identity: Map(id) == id
        /// </summary>
        [TestMethod]
        public void Maybe_FunctorIdentity() =>
            Prop.ForAll<int>(a => Maybe<int>.Some(a).Map(x => x).Equals(Maybe<int>.Some(a))).QuickCheckThrowOnFailure();

        /// <summary>
        /// Functor composition: Map(g ∘ f) == Map(f) ∘ Map(g)
        /// </summary>
        [TestMethod]
        public void Maybe_FunctorComposition() =>
            Prop.ForAll<int>(a =>
            {
                Func<int, int> f = x => x * 2;
                Func<int, int> g = x => x + 1;
                return Maybe<int>.Some(a).Map(f).Map(g).Equals(Maybe<int>.Some(a).Map(x => g(f(x))));
            }).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Result_FunctorIdentity() =>
            Prop.ForAll<int>(a => Result<int>.Success(a).Map(x => x).Equals(Result<int>.Success(a))).QuickCheckThrowOnFailure();

        [TestMethod]
        public void Either_FunctorIdentity() =>
            Prop.ForAll<int>(a => Either<string, int>.FromRight(a).Map(x => x).Equals(Either<string, int>.FromRight(a))).QuickCheckThrowOnFailure();
    }
}
